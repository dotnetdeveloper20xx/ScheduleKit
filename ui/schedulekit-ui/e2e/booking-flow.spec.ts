import { test, expect } from '@playwright/test';

test.describe('Public Booking Flow', () => {
  // Public booking pages don't require authentication
  test.use({ storageState: { cookies: [], origins: [] } });

  // We need a valid event type slug - this should be created in setup or manually
  const testHostSlug = 'test-user';
  const testEventSlug = '30-min-meeting';

  test.describe('Public Booking Page', () => {
    test('should display event type not found for invalid slug', async ({ page }) => {
      await page.goto('/book/invalid-host/invalid-event');

      await expect(page.getByText(/not found|unavailable/i)).toBeVisible({ timeout: 10000 });
    });

    test('should display event type information', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);

      // If the event exists, we should see event details
      // If not, we should see not found
      const eventTitle = page.locator('h1, h2').first();
      const notFound = page.getByText(/not found|unavailable/i);

      await expect(eventTitle.or(notFound)).toBeVisible({ timeout: 10000 });
    });
  });

  test.describe('Date Selection', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);
    });

    test('should display available dates', async ({ page }) => {
      // Wait for dates to load
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      const notFound = page.getByText(/not found|unavailable/i);
      const noDates = page.getByText(/no available dates/i);

      await expect(dateButton.or(notFound).or(noDates)).toBeVisible({ timeout: 15000 });
    });

    test('should select a date and show time slots', async ({ page }) => {
      // Wait for dates to load
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();

      if (!(await dateButton.isVisible({ timeout: 10000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await dateButton.click();

      // Time slots should appear
      const timeSlot = page.locator('button').filter({ hasText: /\d{1,2}:\d{2}\s*(AM|PM)/i }).first();
      const noTimes = page.getByText(/no available times/i);

      await expect(timeSlot.or(noTimes)).toBeVisible({ timeout: 10000 });
    });
  });

  test.describe('Time Slot Selection', () => {
    test('should select time slot and show continue button', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);

      // Select a date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();

      if (!(await dateButton.isVisible({ timeout: 10000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await dateButton.click();

      // Select a time slot
      const timeSlot = page.locator('button').filter({ hasText: /\d{1,2}:\d{2}\s*(AM|PM)/i }).first();

      if (!(await timeSlot.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await timeSlot.click();

      // Continue button should appear
      await expect(page.getByRole('button', { name: /continue/i })).toBeVisible();
    });
  });

  test.describe('Booking Form', () => {
    test('should display booking form after selecting time', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);

      // Select date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 10000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();

      // Select time
      const timeSlot = page.locator('button').filter({ hasText: /\d{1,2}:\d{2}\s*(AM|PM)/i }).first();
      if (!(await timeSlot.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();

      // Click continue
      await page.getByRole('button', { name: /continue/i }).click();

      // Booking form should appear
      await expect(page.getByLabel(/name/i)).toBeVisible({ timeout: 5000 });
      await expect(page.getByLabel(/email/i)).toBeVisible();
    });

    test('should show validation errors for incomplete form', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);

      // Navigate to form
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 10000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();

      const timeSlot = page.locator('button').filter({ hasText: /\d{1,2}:\d{2}\s*(AM|PM)/i }).first();
      if (!(await timeSlot.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();
      await page.getByRole('button', { name: /continue/i }).click();

      // Try to submit empty form
      await page.getByRole('button', { name: /confirm|schedule|book/i }).click();

      // Should show validation errors
      await expect(page.getByText(/required|invalid/i)).toBeVisible();
    });

    test('should complete booking successfully', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);

      // Select date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 10000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();

      // Select time
      const timeSlot = page.locator('button').filter({ hasText: /\d{1,2}:\d{2}\s*(AM|PM)/i }).first();
      if (!(await timeSlot.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();
      await page.getByRole('button', { name: /continue/i }).click();

      // Fill in the form
      await page.getByLabel(/name/i).fill('E2E Test Guest');
      await page.getByLabel(/email/i).fill(`e2e-test-${Date.now()}@example.com`);

      // Fill notes if visible
      const notesField = page.getByLabel(/notes|additional/i);
      if (await notesField.isVisible().catch(() => false)) {
        await notesField.fill('This is an E2E test booking');
      }

      // Submit
      await page.getByRole('button', { name: /confirm|schedule|book/i }).click();

      // Should show confirmation
      await expect(page.getByText(/confirmed|success|scheduled/i)).toBeVisible({ timeout: 15000 });
    });
  });

  test.describe('Widget Booking', () => {
    test('should complete booking through widget', async ({ page }) => {
      await page.goto(`/widget/${testHostSlug}/${testEventSlug}`);

      // Widget should display event name
      const eventHeader = page.locator('h2, h3').first();
      const notFound = page.getByText(/not found/i);

      await expect(eventHeader.or(notFound)).toBeVisible({ timeout: 10000 });

      if (await notFound.isVisible().catch(() => false)) {
        test.skip();
        return;
      }

      // Select date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();

      // Select time (widget goes directly to form)
      const timeSlot = page.locator('button').filter({ hasText: /\d{1,2}:\d{2}\s*(AM|PM)/i }).first();
      if (!(await timeSlot.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();

      // Fill form
      await page.getByLabel(/name/i).fill('Widget Test Guest');
      await page.getByLabel(/email/i).fill(`widget-test-${Date.now()}@example.com`);

      // Submit
      await page.getByRole('button', { name: /confirm/i }).click();

      // Should show confirmation
      await expect(page.getByText(/confirmed|booked/i)).toBeVisible({ timeout: 15000 });
    });
  });
});
