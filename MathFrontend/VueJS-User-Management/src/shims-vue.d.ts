/* eslint-disable */
declare module "*.vue" {
  import type { DefineComponent } from "vue";
  const component: DefineComponent<{}, {}, any>;
  export default component;
}

declare module "bootstrap" {
  export class Modal {
    constructor(element: HTMLElement, options?: any);
    show(): void;
    hide(): void;
    toggle(): void;
    dispose(): void;
  }
}

import "vue";

declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $store: any;
    $router: any;
    $toast: {
      success(message: string): void;
      error(message: string): void;
      warning(message: string): void;
      info(message: string): void;
    };
  }
}
