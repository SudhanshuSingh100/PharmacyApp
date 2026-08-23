module.exports = {
  root: true,
  env: {
    browser: true,
    es2021: true,
    node: true,
  },
  extends: [
    "eslint:recommended",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended",
  ],
  parserOptions: {
    ecmaVersion: "latest",
    sourceType: "module",
    ecmaFeatures: {
      jsx: true,
    },
  },
  settings: {
    react: {
      version: "detect",
    },
  },
  rules: {
    // Example rules you can tweak:
    "react/prop-types": "off", // disable if you don’t use PropTypes
    "no-unused-vars": ["warn", { argsIgnorePattern: "^_" }],
    "react/jsx-uses-react": "off", // not needed in React 17+
    "react/react-in-jsx-scope": "off", // not needed in React 17+
  },
};
