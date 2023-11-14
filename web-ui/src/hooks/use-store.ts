import React from "react";
import { rootContext } from "../stores/root-context";

export const useStores = () => React.useContext(rootContext);
