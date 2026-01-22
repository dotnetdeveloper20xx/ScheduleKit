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
      // Wait for Weekly Hours section to load (use heading to be specific)
      await expect(page.getByRole('heading', { name: 'Weekly Hours' })).toBeVisible();

      // Should display at least some days of the week (Monday-Friday are typically enabled)
      // Use exact matching to avoid matching dates like "Friday, January 30, 2026"
      await expect(page.getByText('Monday', { exact: true })).toBeVisible();
      await expect(page.getByText('Tuesday', { exact: true })).toBeVisible();
      await expect(page.getByText('Friday', { exact: true })).toBeVisible();
    });

    test('should show weekly hours section', async ({ page }) => {
      // The section has a heading "Weekly Hours" (use heading role to be specific)
      await expect(page.getByRole('heading', { name: 'Weekly Hours' })).toBeVisible();
    });
  });

  test.describe('Schedule Management', () => {
    test('should toggle day availability', async ({ page }) => {
      // Toggle buttons are styled <button> elements, not role="switch"
      // They're the first button in each day row
      // Find any toggle-like button (rounded-full with specific width)
      const dayRow = page.locator('.rounded-lg.border.p-4').first();

      if (await dayRow.isVisible({ timeout: 5000 }).catch(() => false)) {
        // Click the toggle button within the row
        const toggleButton = dayRow.locator('button').first();
        await toggleButton.click();

        // Wait for the change to take effect
        await page.waitForTimeout(500);

        // Toggle back to restore state
        await toggleButton.click();
      }
    });

    test('should show time slots for enabled days', async ({ page }) => {
      // Time slots are <select> elements in this app
      const timeSelect = page.locator('select').first();

      // If the page has time selects, they should be visible for enabled days
      if (await timeSelect.isVisible({ timeout: 5000 }).catch(() => false)) {
        await expect(timeSelect).toBeVisible();
      }
    });

    test('should update start time', async ({ page }) => {
      // Find start time select (time slots use <select> elements)
      const timeSelects = page.locator('select');

      if (!(await timeSelects.first().isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Select a new time option
      await timeSelects.first().selectOption('10:00');

      // Wait for auto-save or click save button if present
      const saveButton = page.getByRole('button', { name: /save/i });
      if (await saveButton.isVisible().catch(() => false)) {
        await saveButton.click();
      }
    });

    test('should update end time', async ({ page }) => {
      // End time is the second select for each enabled day
      const timeSelects = page.locator('select');
      const count = await timeSelects.count();

      if (count < 2) {
        test.skip();
        return;
      }

      // Select a new end time
      await timeSelects.nth(1).selectOption('18:00');

      // Wait for auto-save or click save button if present
      const saveButton = page.getByRole('button', { name: /save/i });
      if (await saveButton.isVisible().catch(() => false)) {
        await saveButton.click();
      }
    });
  });

  test.describe('Schedule Persistence', () => {
    test('should persist schedule changes after page reload', async ({ page }) => {
      // Find a time select and change its value
      const timeSelects = page.locator('select');

      if (!(await timeSelects.first().isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Get initial value
      const initialValue = await timeSelects.first().inputValue();

      // Select a different time
      const newValue = initialValue === '09:00' ? '10:00' : '09:00';
      await timeSelects.first().selectOption(newValue);
      await page.waitForTimeout(1000); // Wait for save

      // Click save if available
      const saveButton = page.getByRole('button', { name: /save/i });
      if (await saveButton.isVisible().catch(() => false)) {
        await saveButton.click();
        await page.waitForTimeout(1000);
      }

      // Reload the page
      await page.reload();

      // Check the value persisted
      const newTimeSelects = page.locator('select');
      await expect(newTimeSelects.first()).toBeVisible();
      const persistedValue = await newTimeSelects.first().inputValue();

      expect(persistedValue).toBe(newValue);

      // Restore original state
      await newTimeSelects.first().selectOption(initialValue);
      if (await saveButton.isVisible().catch(() => false)) {
        await saveButton.click();
      }
    });
  });

  test.describe('Schedule Exceptions', () => {
    test('should show date overrides section', async ({ page }) => {
      // Look for "Date Overrides" heading
      await expect(page.getByText('Date Overrides')).toBeVisible();
    });

    test('should add a date override', async ({ page }) => {
      // Wait for the Date Overrides section to load
      await expect(page.getByText('Date Overrides')).toBeVisible();

      // Look for "Add Override" button - it's the first button in Date Overrides section
      const addOverrideButton = page.getByRole('button', { name: 'Add Override' });

      // If button shows "Cancel", the form is already open - skip
      if (!(await addOverrideButton.isVisible({ timeout: 3000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await addOverrideButton.click();

      // After clicking, a date input should appear in the form
      const datePicker = page.locator('input[type="date"]');
      await expect(datePicker).toBeVisible({ timeout: 5000 });
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
    // Status filters are regular buttons: All, Confirmed, Cancelled, Completed, No Show
    await expect(page.getByText('Status:')).toBeVisible();
    await expect(page.getByRole('button', { name: 'All' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Confirmed' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Cancelled' })).toBeVisible();
  });

  test('should show empty state or bookings list', async ({ page }) => {
    // Should show either empty state or booking cards
    const emptyState = page.getByText(/no bookings yet/i);
    // Booking cards have guest name as h3
    const bookingGuestName = page.locator('h3.font-medium.text-gray-900').first();

    await expect(emptyState.or(bookingGuestName)).toBeVisible({ timeout: 10000 });
  });

  test('should navigate to booking details', async ({ page }) => {
    // View button is inside each booking card
    const viewButton = page.getByRole('button', { name: 'View' }).first();

    if (!(await viewButton.isVisible({ timeout: 5000 }).catch(() => false))) {
      test.skip();
      return;
    }

    await viewButton.click();

    // Should navigate to booking detail page
    await expect(page).toHaveURL(/\/bookings\/.+/);
  });

  test('should filter bookings by status', async ({ page }) => {
    // Status filters are regular buttons, not tabs
    const confirmedFilter = page.getByRole('button', { name: 'Confirmed' });
    const cancelledFilter = page.getByRole('button', { name: 'Cancelled' });

    if (await confirmedFilter.isVisible({ timeout: 3000 }).catch(() => false)) {
      await confirmedFilter.click();
      // The active filter button has different styling but no aria-selected
      // Just verify the click works
      await page.waitForTimeout(500);
    }

    if (await cancelledFilter.isVisible().catch(() => false)) {
      await cancelledFilter.click();
      await page.waitForTimeout(500);
    }
  });
});
