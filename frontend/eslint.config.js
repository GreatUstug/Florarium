import js from '@eslint/js';
import globals from 'globals';
import reactHooks from 'eslint-plugin-react-hooks';
import reactRefresh from 'eslint-plugin-react-refresh';
import tseslint from 'typescript-eslint';
import prettier from 'eslint-config-prettier';

const HEX = '/#[0-9a-fA-F]{3,8}\\b/';
const PX = '/\\d(\\.\\d+)?px\\b/';

const exportRules = [
  {
    selector: 'ExportNamedDeclaration[declaration]',
    message: 'inline-экспорт запрещён: объявите, затем export { … } в конце файла',
  },
  {
    selector: 'ExportDefaultDeclaration > :matches(FunctionDeclaration, ClassDeclaration)',
    message: 'inline-экспорт запрещён: объявите, затем export default <имя>',
  },
];

const designSystemRules = [
  { selector: `Literal[value=${HEX}]`, message: 'сырой hex запрещён: используйте var(--color-…)' },
  {
    selector: `TemplateElement[value.raw=${HEX}]`,
    message: 'сырой hex запрещён: используйте var(--color-…)',
  },
  { selector: `Literal[value=${PX}]`, message: 'px запрещены: используйте var(--space-…) и др.' },
  {
    selector: `TemplateElement[value.raw=${PX}]`,
    message: 'px запрещены: используйте var(--space-…) и др.',
  },
  {
    selector: 'Property[key.name="fontFamily"]',
    message: 'шрифты задаются только токенами --font-* в CSS',
  },
];

export default tseslint.config(
  { ignores: ['dist', 'playwright-report', 'test-results'] },
  {
    extends: [js.configs.recommended, ...tseslint.configs.strict],
    files: ['**/*.{js,ts,tsx}'],
    languageOptions: { ecmaVersion: 2022, globals: globals.browser },
    plugins: { 'react-hooks': reactHooks, 'react-refresh': reactRefresh },
    rules: {
      ...reactHooks.configs.recommended.rules,
      'react-refresh/only-export-components': ['warn', { allowConstantExport: true }],
      'no-restricted-syntax': ['error', ...exportRules, ...designSystemRules],
      '@typescript-eslint/no-empty-object-type': [
        'error',
        { allowInterfaces: 'with-single-extends' },
      ],
    },
  },
  {
    files: ['*.config.{js,ts}', 'e2e/**'],
    rules: { 'no-restricted-syntax': ['error', ...exportRules] },
  },
  prettier,
  {
    rules: { curly: ['error', 'all'] },
  },
);
