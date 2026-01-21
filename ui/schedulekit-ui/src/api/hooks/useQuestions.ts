import { useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient, getErrorMessage } from '../client';
import { eventTypeKeys } from './useEventTypes';
import type {
  BookingQuestionResponse,
  AddBookingQuestionRequest,
  UpdateBookingQuestionRequest,
  ReorderQuestionsRequest,
} from '../types';

// Add a booking question
export function useAddQuestion() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      eventTypeId,
      data,
    }: {
      eventTypeId: string;
      data: AddBookingQuestionRequest;
    }) => {
      const response = await apiClient.post<BookingQuestionResponse>(
        `/event-types/${eventTypeId}/questions`,
        data
      );
      return response.data;
    },
    onSuccess: (_, { eventTypeId }) => {
      // Invalidate the event type to refetch with new question
      queryClient.invalidateQueries({
        queryKey: eventTypeKeys.detail(eventTypeId),
      });
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error('Failed to add question:', getErrorMessage(error));
    },
  });
}

// Update a booking question
export function useUpdateQuestion() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      eventTypeId,
      questionId,
      data,
    }: {
      eventTypeId: string;
      questionId: string;
      data: UpdateBookingQuestionRequest;
    }) => {
      const response = await apiClient.put<BookingQuestionResponse>(
        `/event-types/${eventTypeId}/questions/${questionId}`,
        data
      );
      return response.data;
    },
    onSuccess: (_, { eventTypeId }) => {
      queryClient.invalidateQueries({
        queryKey: eventTypeKeys.detail(eventTypeId),
      });
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error('Failed to update question:', getErrorMessage(error));
    },
  });
}

// Delete a booking question
export function useDeleteQuestion() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      eventTypeId,
      questionId,
    }: {
      eventTypeId: string;
      questionId: string;
    }) => {
      await apiClient.delete(
        `/event-types/${eventTypeId}/questions/${questionId}`
      );
      return { eventTypeId, questionId };
    },
    onSuccess: ({ eventTypeId }) => {
      queryClient.invalidateQueries({
        queryKey: eventTypeKeys.detail(eventTypeId),
      });
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error('Failed to delete question:', getErrorMessage(error));
    },
  });
}

// Reorder booking questions
export function useReorderQuestions() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      eventTypeId,
      data,
    }: {
      eventTypeId: string;
      data: ReorderQuestionsRequest;
    }) => {
      await apiClient.post(
        `/event-types/${eventTypeId}/questions/reorder`,
        data
      );
      return eventTypeId;
    },
    onSuccess: (eventTypeId) => {
      queryClient.invalidateQueries({
        queryKey: eventTypeKeys.detail(eventTypeId),
      });
    },
    onError: (error) => {
      console.error('Failed to reorder questions:', getErrorMessage(error));
    },
  });
}
