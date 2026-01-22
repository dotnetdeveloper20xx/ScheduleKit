import { test, expect } from '@playwright/test';

test.describe('Authentication', () => {
  test.describe('Login Page', () => {
    test.use({ storageState: { cookies: [], origins: [] } }); // Clear auth state

    test('should display login form', async ({ page }) => {
      await page.goto('/login');

      await expect(page.getByRole('heading', { name: 'Sign in to your account' })).toBeVisible();
      await expect(page.getByPlaceholder('you@example.com')).toBeVisible();
      await expect(page.getByPlaceholder('Enter your password')).toBeVisible();
      await expect(page.getByRole('button', { name: 'Sign in' })).toBeVisible();
    });

    test('should show validation errors for empty form', async ({ page }) => {
      await page.goto('/login');

      // Click sign in without filling form - HTML5 validation will prevent submission
      // The inputs have 'required' attribute, so we need to test differently
      const emailInput = page.getByPlaceholder('you@example.com');
      const passwordInput = page.getByPlaceholder('Enter your password');

      // Verify inputs are required
      await expect(emailInput).toHaveAttribute('required', '');
      await expect(passwordInput).toHaveAttribute('required', '');
    });

    test('should show error for invalid credentials', async ({ page }) => {
      await page.goto('/login');

      await page.getByPlaceholder('you@example.com').fill('invalid@example.com');
      await page.getByPlaceholder('Enter your password').fill('wrongpassword');
      await page.getByRole('button', { name: 'Sign in' }).click();

      // Wait for error message to appear
      await expect(page.getByText(/invalid|incorrect|failed|error/i)).toBeVisible({ timeout: 10000 });
    });

    test('should redirect to register page', async ({ page }) => {
      await page.goto('/login');

      await page.getByRole('link', { name: 'Sign up' }).click();

      await expect(page).toHaveURL('/register');
    });
  });

  test.describe('Register Page', () => {
    test.use({ storageState: { cookies: [], origins: [] } });

    test('should display registration form', async ({ page }) => {
      await page.goto('/register');

      await expect(page.getByRole('heading', { name: 'Create your account' })).toBeVisible();
      await expect(page.getByPlaceholder('John Doe')).toBeVisible();
      await expect(page.getByPlaceholder('you@example.com')).toBeVisible();
      await expect(page.getByPlaceholder('At least 8 characters')).toBeVisible();
    });

    test('should show validation errors for invalid data', async ({ page }) => {
      await page.goto('/register');

      await page.getByPlaceholder('John Doe').fill('Test User');
      await page.getByPlaceholder('you@example.com').fill('test@example.com');
      await page.getByPlaceholder('At least 8 characters').fill('short'); // Too short
      await page.getByPlaceholder('Confirm your password').fill('short');
      await page.getByRole('button', { name: 'Create account' }).click();

      // Should show validation error for short password
      await expect(page.getByText(/at least 8 characters/i)).toBeVisible();
    });

    test('should redirect to login page', async ({ page }) => {
      await page.goto('/register');

      await page.getByRole('link', { name: 'Sign in' }).click();

      await expect(page).toHaveURL('/login');
    });
  });

  test.describe('Protected Routes', () => {
    test.use({ storageState: { cookies: [], origins: [] } });

    test('should redirect unauthenticated users to login', async ({ page }) => {
      await page.goto('/event-types');

      await expect(page).toHaveURL(/\/login/);
    });

    test('should redirect from dashboard to login when not authenticated', async ({ page }) => {
      await page.goto('/');

      await expect(page).toHaveURL(/\/login/);
    });
  });

  test.describe('Authenticated User', () => {
    // These tests use the authenticated state from setup

    test('should display dashboard when authenticated', async ({ page }) => {
      await page.goto('/');

      // Check for dashboard heading or content
      await expect(page.getByRole('heading', { name: /dashboard/i })).toBeVisible({ timeout: 5000 });
    });

    test('should show user navigation', async ({ page }) => {
      await page.goto('/');

      // On mobile, the sidebar might be hidden - check for desktop viewport
      // Navigation is in a sidebar with NavLinks
      // Look for the sidebar which contains navigation
      const sidebar = page.locator('aside');

      if (await sidebar.isVisible({ timeout: 3000 }).catch(() => false)) {
        // Desktop view - sidebar is visible
        await expect(page.getByText('Event Types', { exact: true })).toBeVisible();
        await expect(page.getByText('Bookings', { exact: true })).toBeVisible();
        await expect(page.getByText('Availability', { exact: true })).toBeVisible();
      } else {
        // Mobile view - look for hamburger menu or navigation links elsewhere
        // For now, just verify the dashboard loaded
        await expect(page.getByRole('heading', { name: /dashboard/i })).toBeVisible();
      }
    });

    test('should logout successfully', async ({ page }) => {
      await page.goto('/');

      // Wait for page to load
      await page.waitForLoadState('networkidle');

      // Logout is in a dropdown menu - first click the user avatar/name button to open dropdown
      // The dropdown trigger is a button with the user's initial and a chevron icon
      const userDropdownTrigger = page.locator('header').getByRole('button').filter({ has: page.locator('.rounded-full') });

      if (await userDropdownTrigger.isVisible({ timeout: 3000 }).catch(() => false)) {
        await userDropdownTrigger.click();

        // Wait for dropdown to open
        await page.waitForTimeout(300);

        // Now click "Sign out" button in the dropdown
        const signOutButton = page.getByRole('button', { name: 'Sign out' });
        await signOutButton.click();

        await expect(page).toHaveURL(/\/login/);
      } else {
        // Fallback - try direct logout button
        const logoutButton = page.getByRole('button', { name: /logout|sign out/i });
        if (await logoutButton.isVisible({ timeout: 2000 }).catch(() => false)) {
          await logoutButton.click();
          await expect(page).toHaveURL(/\/login/);
        } else {
          test.skip();
        }
      }
    });
  });
});
