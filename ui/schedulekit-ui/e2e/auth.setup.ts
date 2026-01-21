import { test as setup, expect } from '@playwright/test';
import path from 'path';

const authFile = path.join(__dirname, '.auth/user.json');

/**
 * Authentication setup - runs before all tests to create authenticated state.
 * Creates a test user and saves the authentication state to be reused.
 */
setup('authenticate', async ({ page }) => {
  // Use test credentials - these should match your test database
  const testEmail = 'test@example.com';
  const testPassword = 'TestPassword123!';
  const testName = 'Test User';

  // Try to login first
  await page.goto('/login');

  // Fill in login form
  await page.getByLabel('Email').fill(testEmail);
  await page.getByLabel('Password').fill(testPassword);
  await page.getByRole('button', { name: /sign in/i }).click();

  // Check if login succeeded or if we need to register
  try {
    // Wait for successful redirect to dashboard
    await expect(page).toHaveURL('/', { timeout: 5000 });
  } catch {
    // Login failed, try to register
    await page.goto('/register');

    await page.getByLabel('Full Name').fill(testName);
    await page.getByLabel('Email').fill(testEmail);
    await page.getByLabel('Password').fill(testPassword);
    await page.getByRole('button', { name: /create account/i }).click();

    // Wait for redirect to dashboard after registration
    await expect(page).toHaveURL('/');
  }

  // Verify we're authenticated by checking for dashboard elements
  await expect(page.getByText('Dashboard')).toBeVisible();

  // Save authentication state
  await page.context().storageState({ path: authFile });
});
