import { InjectionKey } from "vue";
import { createStore, useStore as baseUseStore, Store } from "vuex";
import user from "./modules/user";
import math from "./modules/math";

// Define your store state types
export interface RootState {
  // Define any root state properties
  appName: string;
  appVersion: string;
}

// Define injection key
export const key: InjectionKey<Store<RootState>> = Symbol();

// Create store
export const store = createStore<RootState>({
  state: {
    appName: "Math Tutor",
    appVersion: "1.0.0",
  },
  modules: {
    user,
    math,
  },
});

// Define typed useStore hook
export function useStore() {
  return baseUseStore(key);
}

export default store;
