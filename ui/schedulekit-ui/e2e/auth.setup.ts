import { test as setup, expect } from '@playwright/test';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const authFile = path.join(__dirname, '.auth/user.json');

/**
 * Authentication setup - runs before all tests to create authenticated state.
 * Creates a test user and saves the authentication state to be reused.
 */
setup('authenticate', async ({ page }) => {
  // Use demo credentials that are seeded in the database
  const testEmail = 'demo@schedulekit.com';
  const testPassword = 'Demo123!';

  // Go to login page
  await page.goto('/login');

  // Wait for page to load
  await expect(page.getByRole('heading', { name: 'Sign in to your account' })).toBeVisible();

  // Fill in login form using placeholders since labels have asterisks
  await page.getByPlaceholder('you@example.com').fill(testEmail);
  await page.getByPlaceholder('Enter your password').fill(testPassword);
  await page.getByRole('button', { name: 'Sign in' }).click();

  // Wait for successful redirect to dashboard
  await expect(page).toHaveURL('/', { timeout: 10000 });

  // Verify we're authenticated by checking for dashboard elements
  await expect(page.getByRole('heading', { name: /dashboard/i })).toBeVisible({ timeout: 5000 });

  // Save authentication state
  await page.context().storageState({ path: authFile });
});
