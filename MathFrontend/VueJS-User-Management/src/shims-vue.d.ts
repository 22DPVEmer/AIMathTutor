/* eslint-disable */
// This file contains type declarations for external libraries
// and Vue custom properties that the TypeScript compiler doesn't infer by default.

declare module "*.vue" {
  import type { DefineComponent } from "vue";
  const component: DefineComponent<{}, {}, any>;
  export default component;
}

// Bootstrap Modal declaration
declare module "bootstrap" {
  export class Modal {
    constructor(element: HTMLElement, options?: any);
    show(): void;
    hide(): void;
    toggle(): void;
    dispose(): void;
    // Add other Modal methods as needed
  }
}

// Extend Vue component instance properties
import "vue";

declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $store: any; // Ideally, you would type this with your Vuex store type
    $router: any; // Ideally, you would type this with vue-router types
    $toast: {
      success(message: string): void;
      error(message: string): void;
      warning(message: string): void;
      info(message: string): void;
    };
  }
}
