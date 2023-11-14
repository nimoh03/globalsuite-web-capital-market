import { images } from "@/theme";
import { Divider, Layout, Menu } from "antd";
import Image from "next/image";
const { Sider } = Layout;
import {
  HomeOutlined,
  SmileOutlined,
  QuestionCircleOutlined,
  LogoutOutlined,
} from "@ant-design/icons";
import { ROUTES } from "@/lib/routes";
import { usePathname, useRouter } from "next/navigation";
import { MenuItem, getMenuItem } from "@/lib/utils";
import { useEffect, useState } from "react";
import { useStores } from "@/hooks/use-store";

type SideBarProps = {
  collapsed?: boolean;
};

const items: MenuItem[] = [
  getMenuItem("Dashboard", ROUTES.dashboard().path, <HomeOutlined />),

  getMenuItem("Accounting", "accounting", <HomeOutlined />, [
    getMenuItem("Customer Deposit", ROUTES.accountingCustomerDeposit().path),
    getMenuItem("Journal Posting", ROUTES.accountingJournalPosting().path),
    getMenuItem("Maintain", ROUTES.accountingMaintain().path),
    getMenuItem("Report", ROUTES.accountingReport().path),
  ]),

  getMenuItem("Capital Market", "capital-market", <HomeOutlined />, [
    getMenuItem("Jobbing", ROUTES.capitalMarketJobbing().path),
    getMenuItem(
      "Stock Market Trading",
      ROUTES.capitalMarketStockMarketTrading().path
    ),
    getMenuItem(
      "Portfolio Management",
      ROUTES.capitalMarketPortfolioManagement().path
    ),
    getMenuItem("Maintain", ROUTES.capitalMarketMaintain().path),
    getMenuItem("Report", ROUTES.capitalMarketReport().path),
  ]),

  getMenuItem("Administrator", "administrator", <HomeOutlined />, [
    getMenuItem("User Maintenance", ROUTES.administratorUserMaintenance().path),
    getMenuItem("Maintain", ROUTES.administratorMaintain().path),
    getMenuItem("Company Setup", ROUTES.administratorCompanySetup().path),
    getMenuItem(
      "End Of Period Processing",
      ROUTES.administratorEndOfPeriodProcessing().path
    ),
  ]),

  getMenuItem("Customer Management", "customer-management", <SmileOutlined />, [
    getMenuItem(
      "Customer Setup",
      ROUTES.customerManagementCustomerSetup().path
    ),
    getMenuItem("Maintain", ROUTES.customerManagementMaintain().path),
    getMenuItem("Report", ROUTES.customerManagementReport().path),
    getMenuItem("Enquiry", ROUTES.customerManagementEnquiry().path),
  ]),

  getMenuItem("Human Resources", "human-resources", <HomeOutlined />, [
    getMenuItem(
      "Employee Management",
      ROUTES.humanResourcesEmployeeManagement().path
    ),
  ]),
];

function extractKeyString(path: string): string {
  const words = path.split("/");
  const extractedWords = words.slice(1, 4);
  return "/" + extractedWords.join("/");
}

export const SideBar = ({ collapsed }: SideBarProps) => {
  const router = useRouter();
  const pathname = usePathname();
  const { authStore } = useStores();

  const [selectedKey, setSelectedKey] = useState<string>("");

  const onMenuClick = (item: MenuItem) => {
    if (!item?.key) return;

    if (item.key === "logout") {
      authStore.logout();
      return;
    }

    router.push(item.key as string);
  };

  useEffect(() => {
    setSelectedKey(getSelectedKey());
  }, [pathname]);

  const getSelectedKey = () => {
    const keyPath = extractKeyString(pathname);

    for (const route of Object.values(ROUTES)) {
      if (keyPath === extractKeyString(route().path)) {
        return route().path;
      }
    }

    return keyPath;
  };

  const getDefaultMenuOpenKey = () => {
    const splitPath = pathname.split("/");
    const key = splitPath[2];

    return key;
  };

  return (
    <Sider
      trigger={null}
      collapsible
      collapsed={collapsed}
      width={250}
      className="fixed bg-gray-700 py-2 px-2 overflow-auto h-vh top-0 left-0 bottom-0 "
    >
      <div className="flex flex-col items-center h-full">
        <Image
          src={images.logo}
          alt="logo"
          className="w-full h-10 object-contain "
        />
        <Divider />
        <Menu
          theme="light"
          mode="inline"
          defaultOpenKeys={[getDefaultMenuOpenKey()]}
          className="bg-transparent border-none"
          items={items}
          onClick={onMenuClick}
          selectedKeys={[selectedKey]}
        />

        <Menu
          theme="light"
          mode="inline"
          className="bg-transparent  border-none mt-auto"
          onClick={onMenuClick}
          items={[
            {
              key: "help",
              label: "Get Help",
              icon: <QuestionCircleOutlined />,
            },
            {
              key: "logout",
              label: "Logout",
              icon: <LogoutOutlined />,
            },
          ]}
        />
      </div>
    </Sider>
  );
};
