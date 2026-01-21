import { test, expect } from '@playwright/test';

test.describe('Event Types', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/event-types');
  });

  test.describe('Event Types List', () => {
    test('should display event types page', async ({ page }) => {
      await expect(page.getByRole('heading', { name: /event types/i })).toBeVisible();
    });

    test('should show create button', async ({ page }) => {
      await expect(page.getByRole('link', { name: /new event type/i })).toBeVisible();
    });

    test('should show empty state when no event types', async ({ page }) => {
      // If no event types exist, should show empty state
      const emptyState = page.getByText(/no event types yet/i);
      const eventTypeCard = page.locator('[data-testid="event-type-card"]').first();

      // Either empty state or event type cards should be visible
      await expect(emptyState.or(eventTypeCard)).toBeVisible({ timeout: 10000 });
    });
  });

  test.describe('Create Event Type', () => {
    test('should navigate to create page', async ({ page }) => {
      await page.getByRole('link', { name: /new event type/i }).click();

      await expect(page).toHaveURL('/event-types/new');
      await expect(page.getByRole('heading', { name: /create event type/i })).toBeVisible();
    });

    test('should display event type form', async ({ page }) => {
      await page.goto('/event-types/new');

      await expect(page.getByLabel(/name/i)).toBeVisible();
      await expect(page.getByLabel(/duration/i)).toBeVisible();
      await expect(page.getByLabel(/description/i)).toBeVisible();
    });

    test('should show validation errors for empty form', async ({ page }) => {
      await page.goto('/event-types/new');

      await page.getByRole('button', { name: /create|save/i }).click();

      // Should show validation error for name
      await expect(page.getByText(/name.*required/i)).toBeVisible();
    });

    test('should create event type successfully', async ({ page }) => {
      await page.goto('/event-types/new');

      const uniqueName = `E2E Test Meeting ${Date.now()}`;

      // Fill in the form
      await page.getByLabel(/^name$/i).fill(uniqueName);
      await page.getByLabel(/description/i).fill('E2E test description');

      // Select duration (30 minutes)
      const durationSelect = page.getByLabel(/duration/i);
      await durationSelect.selectOption('30');

      // Select location type
      const locationSelect = page.getByLabel(/location/i);
      if (await locationSelect.isVisible()) {
        await locationSelect.selectOption('Zoom');
      }

      // Submit the form
      await page.getByRole('button', { name: /create|save/i }).click();

      // Should redirect to event types list
      await expect(page).toHaveURL('/event-types', { timeout: 10000 });

      // Should show the new event type
      await expect(page.getByText(uniqueName)).toBeVisible();
    });
  });

  test.describe('Edit Event Type', () => {
    test('should navigate to edit page from card', async ({ page }) => {
      // First create an event type to edit
      const eventTypeCard = page.locator('.card, [data-testid="event-type-card"]').first();

      // Skip if no event types exist
      if (!(await eventTypeCard.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Click edit button
      await eventTypeCard.getByRole('link', { name: /edit/i }).click();

      await expect(page).toHaveURL(/\/event-types\/.*\/edit/);
    });

    test('should update event type name', async ({ page }) => {
      const eventTypeCard = page.locator('.card, [data-testid="event-type-card"]').first();

      if (!(await eventTypeCard.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      await eventTypeCard.getByRole('link', { name: /edit/i }).click();
      await expect(page).toHaveURL(/\/event-types\/.*\/edit/);

      const updatedName = `Updated Event ${Date.now()}`;
      await page.getByLabel(/^name$/i).fill(updatedName);
      await page.getByRole('button', { name: /save|update/i }).click();

      await expect(page).toHaveURL('/event-types', { timeout: 10000 });
      await expect(page.getByText(updatedName)).toBeVisible();
    });
  });

  test.describe('Delete Event Type', () => {
    test('should delete event type with confirmation', async ({ page }) => {
      const eventTypeCard = page.locator('.card, [data-testid="event-type-card"]').first();

      if (!(await eventTypeCard.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Get the name before deletion for verification
      const eventTypeName = await eventTypeCard.locator('h3').textContent();

      // Listen for the confirmation dialog
      page.on('dialog', async (dialog) => {
        expect(dialog.type()).toBe('confirm');
        await dialog.accept();
      });

      // Click delete button
      await eventTypeCard.getByRole('button', { name: /delete/i }).click();

      // The event type should be removed
      if (eventTypeName) {
        await expect(page.getByText(eventTypeName)).not.toBeVisible({ timeout: 5000 });
      }
    });

    test('should cancel deletion', async ({ page }) => {
      const eventTypeCard = page.locator('.card, [data-testid="event-type-card"]').first();

      if (!(await eventTypeCard.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      const eventTypeName = await eventTypeCard.locator('h3').textContent();

      // Dismiss the confirmation dialog
      page.on('dialog', async (dialog) => {
        await dialog.dismiss();
      });

      await eventTypeCard.getByRole('button', { name: /delete/i }).click();

      // The event type should still be visible
      if (eventTypeName) {
        await expect(page.getByText(eventTypeName)).toBeVisible();
      }
    });
  });

  test.describe('Event Type Actions', () => {
    test('should copy booking link', async ({ page, context }) => {
      const eventTypeCard = page.locator('.card, [data-testid="event-type-card"]').first();

      if (!(await eventTypeCard.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Grant clipboard permissions
      await context.grantPermissions(['clipboard-read', 'clipboard-write']);

      // Click copy link button
      await eventTypeCard.getByRole('button', { name: /copy.*link/i }).click();

      // Check clipboard content
      const clipboardContent = await page.evaluate(() => navigator.clipboard.readText());
      expect(clipboardContent).toContain('/book/');
    });

    test('should open embed modal', async ({ page }) => {
      const eventTypeCard = page.locator('.card, [data-testid="event-type-card"]').first();

      if (!(await eventTypeCard.isVisible({ timeout: 5000 }).catch(() => false))) {
        test.skip();
        return;
      }

      // Click embed button (code icon)
      const embedButton = eventTypeCard.getByRole('button', { name: /embed/i });
      if (await embedButton.isVisible().catch(() => false)) {
        await embedButton.click();

        // Modal should appear
        await expect(page.getByText(/embed.*widget/i)).toBeVisible();
        await expect(page.getByText(/iframe/i)).toBeVisible();
      }
    });
  });
});
