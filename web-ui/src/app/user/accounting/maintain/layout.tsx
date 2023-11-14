"use client";

import { ROUTES } from "@/lib/routes";
import { MenuItem, renderSidebarMainMenuItems } from "@/lib/utils";
import { Layout, Menu } from "antd";
import { usePathname, useRouter } from "next/navigation";

const { Content } = Layout;

export default function AccountingLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const pathname = usePathname();

  const onMenuClick = (item: MenuItem) => {
    if (!item?.key) return;

    router.push(item.key as string);
  };

  return (
    <Layout>
      <Menu
        mode="horizontal"
        selectedKeys={[pathname]}
        onClick={onMenuClick}
        items={renderSidebarMainMenuItems(ROUTES.accountingMaintain())}
        className="sticky top-0 z-10"
      />
      <Content className="pt-[50px]">{children}</Content>
    </Layout>
  );
}
