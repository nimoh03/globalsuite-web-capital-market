"use client";
import { Button, Layout } from "antd";
import { SideBar } from "./components/SideBar";
import { useState } from "react";
import UserAppHeader from "./components/UserAppHeader";
import { usePathname, useRouter } from "next/navigation";
import { ROUTES, getRouteTitle } from "@/lib/routes";
import { observer } from "mobx-react-lite";
import { useStores } from "@/hooks/use-store";
import { MenuUnfoldOutlined, MenuFoldOutlined } from "@ant-design/icons";

const { Content } = Layout;

export default observer(function UserLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { authStore } = useStores();
  const router = useRouter();

  const [collapsed, setCollapsed] = useState(false);

  const pathname = usePathname();

  if (typeof window !== "undefined" && !authStore.isAuthenticated) {
    router.push(ROUTES.login().path);
    return null;
  }

  return (
    <Layout hasSider>
      <SideBar collapsed={collapsed} />
      <Layout className="flex" style={{ marginLeft: collapsed ? 80 : 250 }}>
        {/* <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={() => {
            setCollapsed(!collapsed);
          }}
          className="w-[64px] h-[64px] sticky top-0 z-20 px-5"
        /> */}
        <Content className="pr-5">{children}</Content>
      </Layout>
    </Layout>
  );
});
