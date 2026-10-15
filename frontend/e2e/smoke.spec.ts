import { expect, test } from '@playwright/test';

test('главная открывается с шапкой и заголовком', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('link', { name: 'флорариум.' })).toBeVisible();
  await expect(page.getByRole('heading', { level: 1 })).toHaveText('каталог');
});
