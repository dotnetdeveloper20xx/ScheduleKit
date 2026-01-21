import { PageContainer, PageHeader } from '@/components/layout/PageContainer';
import { WeeklySchedule } from './WeeklySchedule';
import { DateOverrides } from './DateOverrides';

export function AvailabilityPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Availability"
        description="Set your weekly hours and manage date-specific exceptions."
      />

      <div className="space-y-6">
        <WeeklySchedule />
        <DateOverrides />
      </div>
    </PageContainer>
  );
}
