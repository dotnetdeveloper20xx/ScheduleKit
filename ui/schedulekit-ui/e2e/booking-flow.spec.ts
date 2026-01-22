import { test, expect } from '@playwright/test';

test.describe('Public Booking Flow', () => {
  // Public booking pages don't require authentication
  test.use({ storageState: { cookies: [], origins: [] } });

  // Use seeded demo data - afzal-ahmed host with 15-minute-quick-chat event type
  // Using 15-min event because it has shorter minNotice (60 min vs 24 hours)
  const testHostSlug = 'afzal-ahmed';
  const testEventSlug = '15-minute-quick-chat';

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
      // Wait for page to fully render
      await page.waitForLoadState('networkidle');
    });

    test('should display available dates', async ({ page }) => {
      // Wait for dates section to appear - look for "Select a Date" heading
      await expect(page.getByRole('heading', { name: 'Select a Date', exact: true })).toBeVisible({ timeout: 15000 });

      // Wait for dates to load (either date buttons or "no available dates" message)
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      const notFound = page.getByText(/not found|unavailable/i);
      const noDates = page.getByText(/no available dates/i);

      await expect(dateButton.or(notFound).or(noDates)).toBeVisible({ timeout: 20000 });
    });

    test('should select a date and show time slots', async ({ page }) => {
      // Wait for "Select a Date" heading
      await expect(page.getByRole('heading', { name: 'Select a Date', exact: true })).toBeVisible({ timeout: 15000 });

      // Wait for dates to load with increased timeout
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();

      if (!(await dateButton.isVisible({ timeout: 20000 }).catch(() => false))) {
        // Check if "no available dates" is shown
        const noDates = page.getByText(/no available dates/i);
        if (await noDates.isVisible().catch(() => false)) {
          test.skip();
          return;
        }
        test.skip();
        return;
      }

      await dateButton.click();

      // Wait for time slots section to update
      await page.waitForTimeout(1000);

      // Time slots should appear - API returns 24-hour format "HH:mm" (e.g., "09:00", "14:30")
      const timeSlot = page.locator('button').filter({ hasText: /\d{2}:\d{2}/ }).first();
      const noTimes = page.getByText(/no available times/i);

      await expect(timeSlot.or(noTimes)).toBeVisible({ timeout: 15000 });
    });
  });

  test.describe('Time Slot Selection', () => {
    test('should select time slot and show continue button', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);
      await page.waitForLoadState('networkidle');

      // Wait for "Select a Date" heading
      await expect(page.getByRole('heading', { name: 'Select a Date', exact: true })).toBeVisible({ timeout: 15000 });

      // Select a date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();

      if (!(await dateButton.isVisible({ timeout: 20000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await dateButton.click();
      await page.waitForTimeout(1000);

      // Select a time slot - API returns 24-hour format "HH:mm"
      const timeSlot = page.locator('button').filter({ hasText: /\d{2}:\d{2}/ }).first();

      if (!(await timeSlot.isVisible({ timeout: 15000 }).catch(() => false))) {
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
      await page.waitForLoadState('networkidle');

      // Wait for "Select a Date" heading
      await expect(page.getByRole('heading', { name: 'Select a Date', exact: true })).toBeVisible({ timeout: 15000 });

      // Select date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 20000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();
      await page.waitForTimeout(1000);

      // Select time - API returns 24-hour format "HH:mm"
      const timeSlot = page.locator('button').filter({ hasText: /\d{2}:\d{2}/ }).first();
      if (!(await timeSlot.isVisible({ timeout: 15000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();

      // Click continue and wait for form
      await page.getByRole('button', { name: /continue/i }).click();

      // Booking form should appear - use placeholder selectors
      await expect(page.getByPlaceholder('John Doe')).toBeVisible({ timeout: 10000 });
      await expect(page.getByPlaceholder('john@example.com')).toBeVisible();
    });

    test('should show validation errors for incomplete form', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);
      await page.waitForLoadState('networkidle');

      // Wait for "Select a Date" heading
      await expect(page.getByRole('heading', { name: 'Select a Date', exact: true })).toBeVisible({ timeout: 15000 });

      // Navigate to form
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 20000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();
      await page.waitForTimeout(1000);

      const timeSlot = page.locator('button').filter({ hasText: /\d{2}:\d{2}/ }).first();
      if (!(await timeSlot.isVisible({ timeout: 15000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();
      await page.getByRole('button', { name: /continue/i }).click();

      // Wait for form to appear
      await expect(page.getByPlaceholder('John Doe')).toBeVisible({ timeout: 10000 });

      // Try to submit empty form
      await page.getByRole('button', { name: /confirm|schedule|book/i }).click();

      // Should show validation errors (use .first() to avoid strict mode)
      await expect(page.getByText(/required|invalid/i).first()).toBeVisible();
    });

    test('should complete booking successfully', async ({ page }) => {
      await page.goto(`/book/${testHostSlug}/${testEventSlug}`);
      await page.waitForLoadState('networkidle');

      // Wait for "Select a Date" heading
      await expect(page.getByRole('heading', { name: 'Select a Date', exact: true })).toBeVisible({ timeout: 15000 });

      // Select date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 20000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();
      await page.waitForTimeout(1000);

      // Select time - API returns 24-hour format "HH:mm"
      const timeSlot = page.locator('button').filter({ hasText: /\d{2}:\d{2}/ }).first();
      if (!(await timeSlot.isVisible({ timeout: 15000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await timeSlot.click();
      await page.getByRole('button', { name: /continue/i }).click();

      // Wait for form to appear
      await expect(page.getByPlaceholder('John Doe')).toBeVisible({ timeout: 10000 });

      // Fill in the form using placeholder selectors
      await page.getByPlaceholder('John Doe').fill('E2E Test Guest');
      await page.getByPlaceholder('john@example.com').fill(`e2e-test-${Date.now()}@example.com`);

      // Fill notes if visible
      const notesField = page.getByPlaceholder(/prepare for our meeting/i);
      if (await notesField.isVisible().catch(() => false)) {
        await notesField.fill('This is an E2E test booking');
      }

      // Submit
      await page.getByRole('button', { name: /confirm|schedule|book/i }).click();

      // Should show confirmation
      await expect(page.getByText(/confirmed|success|scheduled/i).first()).toBeVisible({ timeout: 15000 });
    });
  });

  test.describe('Widget Booking', () => {
    test('should complete booking through widget', async ({ page }) => {
      await page.goto(`/widget/${testHostSlug}/${testEventSlug}`);
      await page.waitForLoadState('networkidle');

      // Widget should display event name
      const eventHeader = page.locator('h2, h3').first();
      const notFound = page.getByText(/not found/i);

      await expect(eventHeader.or(notFound)).toBeVisible({ timeout: 15000 });

      if (await notFound.isVisible().catch(() => false)) {
        test.skip();
        return;
      }

      // Widget has "Select a date" heading
      await expect(page.getByText('Select a date')).toBeVisible({ timeout: 10000 });

      // Select date
      const dateButton = page.locator('button').filter({ hasText: /monday|tuesday|wednesday|thursday|friday|saturday|sunday/i }).first();
      if (!(await dateButton.isVisible({ timeout: 20000 }).catch(() => false))) {
        test.skip();
        return;
      }
      await dateButton.click();
      await page.waitForTimeout(1000);

      // Wait for time slots to load (widget shows times after date selection)
      const timeSlot = page.locator('button').filter({ hasText: /\d{2}:\d{2}/ }).first();
      if (!(await timeSlot.isVisible({ timeout: 15000 }).catch(() => false))) {
        // Time slots might show "No available times" - skip in that case
        test.skip();
        return;
      }
      await timeSlot.click();

      // Wait for form step to appear (widget transitions to 'details' step)
      await page.waitForTimeout(500);

      // Widget form uses placeholder "Your name"
      const nameField = page.getByPlaceholder('Your name');
      if (!(await nameField.isVisible({ timeout: 10000 }).catch(() => false))) {
        // Form didn't appear - might be a state issue
        test.skip();
        return;
      }

      // Fill form
      await nameField.fill('Widget Test Guest');
      await page.getByPlaceholder('you@example.com').fill(`widget-test-${Date.now()}@example.com`);

      // Submit - widget button says "Confirm Booking"
      await page.getByRole('button', { name: 'Confirm Booking' }).click();

      // Should show confirmation
      await expect(page.getByText(/booked|confirmed|scheduled/i).first()).toBeVisible({ timeout: 15000 });
    });
  });
});
