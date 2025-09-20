import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HttpClientModule } from '@angular/common/http'; // Import HttpClientModule
import { FormsModule } from '@angular/forms'; // Import FormsModule

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(HttpClientModule), // Añade HttpClientModule
    importProvidersFrom(FormsModule) // Añade FormsModule
  ]
};
