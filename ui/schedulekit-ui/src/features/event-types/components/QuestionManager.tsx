import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button, Input, Select, Card, CardContent, type SelectOption } from '@/components/ui';
import {
  useAddQuestion,
  useUpdateQuestion,
  useDeleteQuestion,
  useReorderQuestions,
} from '@/api/hooks/useQuestions';
import type { BookingQuestionResponse, QuestionType } from '@/api/types';

interface QuestionManagerProps {
  eventTypeId: string;
  questions: BookingQuestionResponse[];
}

const questionSchema = z.object({
  questionText: z
    .string()
    .min(1, 'Question text is required')
    .max(500, 'Question text must not exceed 500 characters'),
  type: z.string().min(1, 'Question type is required'),
  isRequired: z.boolean(),
  options: z.string().optional(),
});

type QuestionFormData = z.infer<typeof questionSchema>;

const questionTypeOptions: SelectOption[] = [
  { value: 'Text', label: 'Short Text' },
  { value: 'MultilineText', label: 'Long Text' },
  { value: 'SingleSelect', label: 'Single Choice' },
  { value: 'MultiSelect', label: 'Multiple Choice' },
  { value: 'Checkbox', label: 'Checkbox' },
];

export function QuestionManager({ eventTypeId, questions }: QuestionManagerProps) {
  const [isAddingQuestion, setIsAddingQuestion] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<BookingQuestionResponse | null>(null);

  const addQuestion = useAddQuestion();
  const updateQuestion = useUpdateQuestion();
  const deleteQuestion = useDeleteQuestion();
  const reorderQuestions = useReorderQuestions();

  const {
    register,
    handleSubmit,
    watch,
    reset,
    setValue,
    formState: { errors },
  } = useForm<QuestionFormData>({
    resolver: zodResolver(questionSchema),
    defaultValues: {
      questionText: '',
      type: 'Text',
      isRequired: false,
      options: '',
    },
  });

  const questionType = watch('type');
  const showOptions = questionType === 'SingleSelect' || questionType === 'MultiSelect';

  const handleAddQuestion = (data: QuestionFormData) => {
    const optionsArray = showOptions && data.options
      ? data.options.split('\n').map(o => o.trim()).filter(Boolean)
      : undefined;

    addQuestion.mutate(
      {
        eventTypeId,
        data: {
          questionText: data.questionText,
          type: data.type as QuestionType,
          isRequired: data.isRequired,
          options: optionsArray,
        },
      },
      {
        onSuccess: () => {
          reset();
          setIsAddingQuestion(false);
        },
      }
    );
  };

  const handleUpdateQuestion = (data: QuestionFormData) => {
    if (!editingQuestion) return;

    const optionsArray = showOptions && data.options
      ? data.options.split('\n').map(o => o.trim()).filter(Boolean)
      : undefined;

    updateQuestion.mutate(
      {
        eventTypeId,
        questionId: editingQuestion.id,
        data: {
          questionText: data.questionText,
          type: data.type as QuestionType,
          isRequired: data.isRequired,
          options: optionsArray,
        },
      },
      {
        onSuccess: () => {
          reset();
          setEditingQuestion(null);
        },
      }
    );
  };

  const handleDeleteQuestion = (questionId: string) => {
    if (confirm('Are you sure you want to delete this question?')) {
      deleteQuestion.mutate({ eventTypeId, questionId });
    }
  };

  const handleMoveUp = (index: number) => {
    if (index === 0) return;
    const newOrder = [...questions];
    [newOrder[index - 1], newOrder[index]] = [newOrder[index], newOrder[index - 1]];
    reorderQuestions.mutate({
      eventTypeId,
      data: { questionIds: newOrder.map(q => q.id) },
    });
  };

  const handleMoveDown = (index: number) => {
    if (index === questions.length - 1) return;
    const newOrder = [...questions];
    [newOrder[index], newOrder[index + 1]] = [newOrder[index + 1], newOrder[index]];
    reorderQuestions.mutate({
      eventTypeId,
      data: { questionIds: newOrder.map(q => q.id) },
    });
  };

  const startEditing = (question: BookingQuestionResponse) => {
    setEditingQuestion(question);
    setIsAddingQuestion(false);
    setValue('questionText', question.questionText);
    setValue('type', question.type);
    setValue('isRequired', question.isRequired);
    setValue('options', question.options?.join('\n') ?? '');
  };

  const cancelEditing = () => {
    setEditingQuestion(null);
    setIsAddingQuestion(false);
    reset();
  };

  const startAdding = () => {
    setEditingQuestion(null);
    setIsAddingQuestion(true);
    reset();
  };

  const sortedQuestions = [...questions].sort((a, b) => a.displayOrder - b.displayOrder);

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">
          Booking Questions
        </h3>
        {!isAddingQuestion && !editingQuestion && (
          <Button variant="outline" size="sm" onClick={startAdding}>
            Add Question
          </Button>
        )}
      </div>

      {questions.length === 0 && !isAddingQuestion && (
        <p className="text-sm text-gray-500">
          No custom questions added yet. Add questions to collect additional information from guests.
        </p>
      )}

      {/* Questions List */}
      <div className="space-y-3">
        {sortedQuestions.map((question, index) => (
          <Card key={question.id}>
            <CardContent className="p-4">
              {editingQuestion?.id === question.id ? (
                <form onSubmit={handleSubmit(handleUpdateQuestion)} className="space-y-4">
                  <Input
                    label="Question"
                    placeholder="Enter your question..."
                    error={errors.questionText?.message}
                    {...register('questionText')}
                  />
                  <div className="grid grid-cols-2 gap-4">
                    <Select
                      label="Question Type"
                      options={questionTypeOptions}
                      error={errors.type?.message}
                      {...register('type')}
                    />
                    <div className="flex items-center gap-2 pt-8">
                      <input
                        type="checkbox"
                        id="isRequired"
                        className="h-4 w-4 rounded border-gray-300"
                        {...register('isRequired')}
                      />
                      <label htmlFor="isRequired" className="text-sm text-gray-700">
                        Required
                      </label>
                    </div>
                  </div>
                  {showOptions && (
                    <div>
                      <label className="mb-1 block text-sm font-medium text-gray-700">
                        Options (one per line)
                      </label>
                      <textarea
                        className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
                        rows={4}
                        placeholder="Option 1&#10;Option 2&#10;Option 3"
                        {...register('options')}
                      />
                    </div>
                  )}
                  <div className="flex justify-end gap-2">
                    <Button type="button" variant="outline" size="sm" onClick={cancelEditing}>
                      Cancel
                    </Button>
                    <Button type="submit" size="sm" isLoading={updateQuestion.isPending}>
                      Save Changes
                    </Button>
                  </div>
                </form>
              ) : (
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <div className="flex items-center gap-2">
                      <span className="font-medium text-gray-900">{question.questionText}</span>
                      {question.isRequired && (
                        <span className="rounded-full bg-red-100 px-2 py-0.5 text-xs text-red-700">
                          Required
                        </span>
                      )}
                    </div>
                    <div className="mt-1 flex items-center gap-3 text-sm text-gray-500">
                      <span>
                        {questionTypeOptions.find(o => o.value === question.type)?.label || question.type}
                      </span>
                      {question.options?.length > 0 && (
                        <span>{question.options.length} options</span>
                      )}
                    </div>
                  </div>
                  <div className="flex items-center gap-1">
                    <button
                      type="button"
                      onClick={() => handleMoveUp(index)}
                      disabled={index === 0 || reorderQuestions.isPending}
                      className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600 disabled:opacity-50"
                      title="Move up"
                    >
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 15l7-7 7 7" />
                      </svg>
                    </button>
                    <button
                      type="button"
                      onClick={() => handleMoveDown(index)}
                      disabled={index === questions.length - 1 || reorderQuestions.isPending}
                      className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600 disabled:opacity-50"
                      title="Move down"
                    >
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                      </svg>
                    </button>
                    <button
                      type="button"
                      onClick={() => startEditing(question)}
                      className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
                      title="Edit"
                    >
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                    </button>
                    <button
                      type="button"
                      onClick={() => handleDeleteQuestion(question.id)}
                      disabled={deleteQuestion.isPending}
                      className="rounded p-1 text-gray-400 hover:bg-red-50 hover:text-red-600"
                      title="Delete"
                    >
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Add New Question Form */}
      {isAddingQuestion && (
        <Card>
          <CardContent className="p-4">
            <form onSubmit={handleSubmit(handleAddQuestion)} className="space-y-4">
              <Input
                label="Question"
                placeholder="Enter your question..."
                error={errors.questionText?.message}
                {...register('questionText')}
              />
              <div className="grid grid-cols-2 gap-4">
                <Select
                  label="Question Type"
                  options={questionTypeOptions}
                  error={errors.type?.message}
                  {...register('type')}
                />
                <div className="flex items-center gap-2 pt-8">
                  <input
                    type="checkbox"
                    id="newIsRequired"
                    className="h-4 w-4 rounded border-gray-300"
                    {...register('isRequired')}
                  />
                  <label htmlFor="newIsRequired" className="text-sm text-gray-700">
                    Required
                  </label>
                </div>
              </div>
              {showOptions && (
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">
                    Options (one per line, minimum 2)
                  </label>
                  <textarea
                    className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
                    rows={4}
                    placeholder="Option 1&#10;Option 2&#10;Option 3"
                    {...register('options')}
                  />
                </div>
              )}
              <div className="flex justify-end gap-2">
                <Button type="button" variant="outline" size="sm" onClick={cancelEditing}>
                  Cancel
                </Button>
                <Button type="submit" size="sm" isLoading={addQuestion.isPending}>
                  Add Question
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      {questions.length >= 10 && !isAddingQuestion && (
        <p className="text-sm text-amber-600">
          Maximum of 10 questions reached.
        </p>
      )}
    </div>
  );
}
