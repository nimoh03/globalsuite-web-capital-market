"use client";

import { observer } from "mobx-react-lite";
import { useStores } from "../hooks/use-store";
import { Button } from "antd";
import { useEffect } from "react";
import { ROUTES } from "@/lib/routes";
import { useRouter } from "next/navigation";

export default observer(function Home() {
  const { userStore } = useStores();

  const router = useRouter();

  //hard fix for static deployment on IIS. to fix this later
  useEffect(() => {
    router.push(ROUTES.login().path);
  }, []);

  return null;
});
