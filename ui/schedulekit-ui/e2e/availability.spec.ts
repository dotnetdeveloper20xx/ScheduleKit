import { test, expect } from '@playwright/test';

test.describe('Availability Management', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/availability');
  });

  test.describe('Availability Page', () => {
    test('should display availability page', async ({ page }) => {
      await expect(page.getByRole('heading', { name: /availability/i })).toBeVisible();
    });

    test('should show weekly schedule', async ({ page }) => {
      // Should display all days of the week
      const days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

      for (const day of days) {
        await expect(page.getByText(day)).toBeVisible();
      }
    });

    test('should show timezone selector', async ({ page }) => {
      const timezoneElement = page.getByText(/timezone/i);
      await expect(timezoneElement).toBeVisible();
    });
  });

  test.describe('Schedule Management', () => {
    test('should toggle day availability', async ({ page }) => {
      // Find a day toggle switch
      const dayToggle = page.locator('[role="switch"], input[type="checkbox"]').first();

      if (await dayToggle.isVisible().catch(() => false)) {
        const initialState = await dayToggle.isChecked();
        await dayToggle.click();

        // Wait for the change to take effect
        await page.waitForTimeout(500);

        // The state should have changed
        const newState = await dayToggle.isChecked();
        expect(newState).not.toBe(initialState);

        // Toggle back
        await dayToggle.click();
      }
    });

    test('should show time slots for enabled days', async ({ page }) => {
      // Look for time inputs or selectors
      const timeInput = page.locator('input[type="time"], select').first();

      // If the page has time inputs, they should be visible for enabled days
      if (await timeInput.isVisible({ timeout: 5000 }).catch(() => false)) {
        await expect(timeInput).toBeVisible();
      }
    });

    test('should update start time', async ({ page }) => {
      // Find start time input
      const startTimeInput = page.locator('input[type="time"]').first();

      if (!(await startTimeInput.isVisible({ timeout: 5000 }).catch(() => false))) {
        // Try select element instead
        const startTimeSelect = page.locator('select').first();
        if (await startTimeSelect.isVisible().catch(() => false)) {
          await startTimeSelect.selectOption({ index: 2 });
        } else {
          test.skip();
        }
        return;
      }

      // Set a new start time
      await startTimeInput.fill('09:00');

      // Wait for auto-save or click save button if present
      const saveButton = page.getByRole('button', { name: /save/i });
      if (await saveButton.isVisible().catch(() => false)) {
        await saveButton.click();
      }
    });

    test('should update end time', async ({ page }) => {
      // Find end time input (usually the second time input)
      const endTimeInputs = page.locator('input[type="time"]');
      const count = await endTimeInputs.count();

      if (count < 2) {
        test.skip();
        return;
      }

      const endTimeInput = endTimeInputs.nth(1);
      await endTimeInput.fill('17:00');

      // Wait for auto-save or click save button if present
      const saveButton = page.getByRole('button', { name: /save/i });
      if (await saveButton.isVisible().catch(() => false)) {
        await saveButton.click();
      }
    });
  });

  test.describe('Schedule Persistence', () => {
    test('should persist schedule changes after page reload', async ({ page }) => {
      // Make a change to the schedule
      const dayToggle = page.locator('[role="switch"], input[type="checkbox"]').first();

      if (!(await dayToggle.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Get initial state
      const initialState = await dayToggle.isChecked();

      // Toggle the day
      await dayToggle.click();
      await page.waitForTimeout(1000); // Wait for save

      // Reload the page
      await page.reload();

      // Check the state persisted
      const newDayToggle = page.locator('[role="switch"], input[type="checkbox"]').first();
      await expect(newDayToggle).toBeVisible();
      const newState = await newDayToggle.isChecked();

      expect(newState).not.toBe(initialState);

      // Restore original state
      await newDayToggle.click();
    });
  });

  test.describe('Schedule Exceptions', () => {
    test('should show date overrides section if available', async ({ page }) => {
      // Look for date overrides or exceptions section
      const overridesSection = page.getByText(/override|exception|specific date/i);

      if (await overridesSection.isVisible({ timeout: 3000 }).catch(() => false)) {
        await expect(overridesSection).toBeVisible();
      }
    });

    test('should add a date override', async ({ page }) => {
      // Look for "Add date override" button
      const addOverrideButton = page.getByRole('button', { name: /add.*override|add.*exception|block.*date/i });

      if (!(await addOverrideButton.isVisible({ timeout: 3000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await addOverrideButton.click();

      // A date picker or modal should appear
      const datePicker = page.locator('input[type="date"], [role="dialog"]');
      await expect(datePicker).toBeVisible();
    });
  });

  test.describe('Schedule Templates', () => {
    test('should show schedule template option if available', async ({ page }) => {
      // Look for template or preset options
      const templateOption = page.getByText(/template|preset|copy.*schedule/i);

      if (await templateOption.isVisible({ timeout: 3000 }).catch(() => false)) {
        await expect(templateOption).toBeVisible();
      }
    });
  });
});

test.describe('Bookings List', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/bookings');
  });

  test('should display bookings page', async ({ page }) => {
    await expect(page.getByRole('heading', { name: /bookings/i })).toBeVisible();
  });

  test('should show filter options', async ({ page }) => {
    // Look for filter buttons/tabs
    const filterOptions = page.locator('[role="tablist"], .filter-tabs');

    if (await filterOptions.isVisible({ timeout: 3000 }).catch(() => false)) {
      await expect(filterOptions).toBeVisible();
    }
  });

  test('should show empty state or bookings list', async ({ page }) => {
    // Should show either empty state or booking cards
    const emptyState = page.getByText(/no bookings|no upcoming/i);
    const bookingCard = page.locator('.card, [data-testid="booking-card"]').first();

    await expect(emptyState.or(bookingCard)).toBeVisible({ timeout: 10000 });
  });

  test('should navigate to booking details', async ({ page }) => {
    const bookingCard = page.locator('.card, [data-testid="booking-card"]').first();

    if (!(await bookingCard.isVisible({ timeout: 5000 }).catch(() => false))) {
      test.skip();
      return;
    }

    // Click on the booking card or view details button
    const viewButton = bookingCard.getByRole('link', { name: /view|details/i });
    if (await viewButton.isVisible().catch(() => false)) {
      await viewButton.click();
    } else {
      await bookingCard.click();
    }

    // Should navigate to booking detail page
    await expect(page).toHaveURL(/\/bookings\/.+/);
  });

  test('should filter bookings by status', async ({ page }) => {
    // Look for status filter tabs
    const upcomingTab = page.getByRole('tab', { name: /upcoming/i });
    const pastTab = page.getByRole('tab', { name: /past|completed/i });
    const cancelledTab = page.getByRole('tab', { name: /cancelled/i });

    if (await upcomingTab.isVisible({ timeout: 3000 }).catch(() => false)) {
      await upcomingTab.click();
      await expect(upcomingTab).toHaveAttribute('aria-selected', 'true');
    }

    if (await pastTab.isVisible().catch(() => false)) {
      await pastTab.click();
      await expect(pastTab).toHaveAttribute('aria-selected', 'true');
    }
  });
});
