import { MenuProps } from "antd";

export type MenuItem = Required<MenuProps>["items"][number];

export function getMenuItem(
  label: React.ReactNode,
  key: React.Key,
  icon?: React.ReactNode,
  children?: MenuItem[],
  type?: "group"
): MenuItem {
  return {
    key,
    icon,
    children,
    label,
    type,
  } as MenuItem;
}

export function renderSidebarMainMenuItems(route: any) {
  if (!route.children) return [];

  const menuItems = route.children.map((child: any) => {
    return getMenuItem(child.label, child.path, child.icon);
  });

  return menuItems;
}
