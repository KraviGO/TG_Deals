import React, { createContext, useContext, useMemo } from 'react';

export type AppLanguage = 'ru' | 'en';

export interface LocalizedText {
  ru: string;
  en: string;
}

interface I18nContextValue {
  language: AppLanguage;
  setLanguage: (language: AppLanguage) => void;
  tx: (text: LocalizedText | string) => string;
}

const I18nContext = createContext<I18nContextValue | undefined>(undefined);

export const I18nProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const language: AppLanguage = 'ru';

  const value = useMemo<I18nContextValue>(
    () => ({
      language,
      setLanguage: () => undefined,
      tx: (text) => (typeof text === 'string' ? text : text.ru),
    }),
    []
  );

  return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
};

export const useI18n = () => {
  const ctx = useContext(I18nContext);
  if (!ctx) {
    throw new Error('useI18n must be used inside I18nProvider');
  }
  return ctx;
};
