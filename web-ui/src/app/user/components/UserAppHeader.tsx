import { Button, Dropdown, Layout, MenuProps, Tooltip } from "antd";
import { observer } from "mobx-react-lite";
import { MenuFoldOutlined, MenuUnfoldOutlined } from "@ant-design/icons";
import { SearchOutlined, BellOutlined, DownOutlined } from "@ant-design/icons";
import Image from "next/image";
import { images } from "@/theme";

const { Header } = Layout;

type Props = {
  collapsed?: boolean;
  onCollapseClick?: () => void;
  title?: string;
};

const items: MenuProps["items"] = [
  {
    label: <a href="#">1st menu item</a>,
    key: "0",
  },
  {
    label: <a href="#">2nd menu item</a>,
    key: "1",
  },
  {
    type: "divider",
  },
  {
    label: "3rd menu item",
    key: "3",
  },
];

export default observer(function UserAppHeader({
  collapsed = false,
  title,
  onCollapseClick,
}: Props) {
  return (
    <Header className="bg-white sticky top-0 z-10 w-full flex items-center justify-between pt-12 pb-10 px-4 ">
      <div className="flex">
        <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={onCollapseClick}
          className="w-[64px] h-[64px]"
        />
        <h4 className="ml-4">{title}</h4>
      </div>

      <div className="flex gap-5 items-center">
        <Tooltip title="search">
          <Button shape="circle" icon={<SearchOutlined />} />
        </Tooltip>
        <Tooltip title="search">
          <Button shape="circle" icon={<BellOutlined />} />
        </Tooltip>

        <Dropdown menu={{ items }} trigger={["click"]}>
          <div className="flex items-center bg-gray-50 rounded-full h-[50px] px-4">
            <Image src={images.testUser} alt="user avatar" />
            <span className="mx-2 font-bold">John Doe</span>
            <DownOutlined />
          </div>
        </Dropdown>
      </div>
    </Header>
  );
});
