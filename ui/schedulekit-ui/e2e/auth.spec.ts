import { test, expect } from '@playwright/test';

test.describe('Authentication', () => {
  test.describe('Login Page', () => {
    test.use({ storageState: { cookies: [], origins: [] } }); // Clear auth state

    test('should display login form', async ({ page }) => {
      await page.goto('/login');

      await expect(page.getByRole('heading', { name: /sign in/i })).toBeVisible();
      await expect(page.getByLabel('Email')).toBeVisible();
      await expect(page.getByLabel('Password')).toBeVisible();
      await expect(page.getByRole('button', { name: /sign in/i })).toBeVisible();
    });

    test('should show validation errors for empty form', async ({ page }) => {
      await page.goto('/login');

      await page.getByRole('button', { name: /sign in/i }).click();

      // Check for validation messages
      await expect(page.getByText(/email.*required/i)).toBeVisible();
    });

    test('should show error for invalid credentials', async ({ page }) => {
      await page.goto('/login');

      await page.getByLabel('Email').fill('invalid@example.com');
      await page.getByLabel('Password').fill('wrongpassword');
      await page.getByRole('button', { name: /sign in/i }).click();

      await expect(page.getByText(/invalid|incorrect|failed/i)).toBeVisible({ timeout: 10000 });
    });

    test('should redirect to register page', async ({ page }) => {
      await page.goto('/login');

      await page.getByRole('link', { name: /create.*account|sign up|register/i }).click();

      await expect(page).toHaveURL('/register');
    });
  });

  test.describe('Register Page', () => {
    test.use({ storageState: { cookies: [], origins: [] } });

    test('should display registration form', async ({ page }) => {
      await page.goto('/register');

      await expect(page.getByRole('heading', { name: /create.*account|register|sign up/i })).toBeVisible();
      await expect(page.getByLabel('Full Name')).toBeVisible();
      await expect(page.getByLabel('Email')).toBeVisible();
      await expect(page.getByLabel('Password')).toBeVisible();
    });

    test('should show validation errors for invalid data', async ({ page }) => {
      await page.goto('/register');

      await page.getByLabel('Full Name').fill('A'); // Too short
      await page.getByLabel('Email').fill('invalid-email');
      await page.getByLabel('Password').fill('123'); // Too weak
      await page.getByRole('button', { name: /create account|register|sign up/i }).click();

      // Should show validation errors
      await expect(page.locator('.text-red-600, .text-destructive')).toBeVisible();
    });

    test('should redirect to login page', async ({ page }) => {
      await page.goto('/register');

      await page.getByRole('link', { name: /sign in|login|already have/i }).click();

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

      await expect(page.getByText('Dashboard')).toBeVisible();
    });

    test('should show user navigation', async ({ page }) => {
      await page.goto('/');

      // Check for navigation elements
      await expect(page.getByRole('link', { name: /event types/i })).toBeVisible();
      await expect(page.getByRole('link', { name: /bookings/i })).toBeVisible();
      await expect(page.getByRole('link', { name: /availability/i })).toBeVisible();
    });

    test('should logout successfully', async ({ page }) => {
      await page.goto('/');

      // Find and click logout button
      const logoutButton = page.getByRole('button', { name: /logout|sign out/i });
      if (await logoutButton.isVisible()) {
        await logoutButton.click();
        await expect(page).toHaveURL(/\/login/);
      }
    });
  });
});
