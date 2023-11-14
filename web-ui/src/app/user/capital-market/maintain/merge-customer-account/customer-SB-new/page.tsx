"use client";
import { useState } from "react";
import Link from "next/link";
import {
  DatePicker,
  Form,
  Input,
  Select,
  Button,
  Radio,
  Checkbox,
  Table,
  Tooltip,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import {
  PaperClipOutlined,
  UsergroupDeleteOutlined,
  CloseOutlined,
} from "@ant-design/icons";
import { observer } from "mobx-react-lite";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

//first table
const columns: ColumnsType<DataType> = [
  {
    title: <p className="font-bold text-base leading-6">CUST ID</p>,
    dataIndex: "cust id",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">Surname</p>,
    dataIndex: "surname",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">First Name</p>,
    dataIndex: "first name",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">Other Name</p>,
    dataIndex: "other name",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">CSCS Account</p>,
    dataIndex: "cscs account",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">CSCS CHN</p>,
    dataIndex: "cscs chn",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">Actions</p>,
    key: "action",
    align: "center",
    responsive: ["sm"],
    width: 50,
    render: () => (
      <div className="flex justify-center items-center gap-4">
        <Tooltip placement="bottom" title="edit">
          <Link
            href="/user/capital-market/portfolio-management/Add-To-Portfolio-Holding-Balance/edit"
            className=""
          >
            <UsergroupDeleteOutlined className="text-xl text-black" />
          </Link>
        </Tooltip>
      </div>
    ),
  },
];
interface DataType {
  key: React.Key;
  "cust id": string;
  surname: string;
  "first name": string;
  "other name": string;
  "cscs account": string;
  "cscs chn": string;
}
const data: DataType[] = [
  {
    key: "1",
    "cust id": "10010021",
    surname: "Ayomide",
    "first name": "Ayomide",
    "other name": "Ayomide",
    "cscs account": "234567890",
    "cscs chn": "234567890",
  },
];
const rowSelection = {
  onChange: (selectedRowKeys: React.Key[], selectedRows: DataType[]) => {
    console.log(
      `selectedRowKeys: ${selectedRowKeys}`,
      "selectedRows: ",
      selectedRows
    );
  },
};

//first table

const columns2: ColumnsType<ataType> = [
  {
    title: <p className="font-bold text-base leading-6">CUST ID</p>,
    dataIndex: "cust id",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">Surname</p>,
    dataIndex: "surname",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">CSCS Account</p>,
    dataIndex: "cscs account",
    align: "center",
  },
  {
    title: <p className="font-bold text-base leading-6">Actions</p>,
    key: "action",
    align: "center",
    responsive: ["sm"],
    width: 50,
    render: () => (
      <div className="flex justify-center items-center gap-4">
        <Tooltip placement="bottom" title="edit">
          <Link
            href="/user/capital-market/portfolio-management/Add-To-Portfolio-Holding-Balance/edit"
            className=""
          >
            <CloseOutlined className="text-xl text-black" />
          </Link>
        </Tooltip>
      </div>
    ),
  },
];
interface ataType {
  key: React.Key;
  "cust id": string;
  surname: string;
  "cscs account": string;
}
const data2: ataType[] = [
  {
    key: "1",
    "cust id": "10010021",
    surname: "Ayomide",
    "cscs account": "234567890",
  },
];
const rowSelection2 = {
  onChange: (selectedRowKeys: React.Key[], selectedRows: ataType[]) => {
    console.log(
      `selectedRowKeys: ${selectedRowKeys}`,
      "selectedRows: ",
      selectedRows
    );
  },
};

export default observer(function Page() {
  const [selectionType, setSelectionType] = useState<"checkbox" | "radio">(
    "checkbox"
  );
  return (
    <div className={manrope.className}>
      <main className="min-h-screen m-2 ">
        <Form className="rounded-xl border overflow-hidden mx-auto shadow-sm bg-gray-50">
          <h4 className="bg-blue-600 p-2 pl-10 text-white text-base">
            <PaperClipOutlined />
            Merge Customer StockBroking Account - New
          </h4>
          <div className="py-5 md:px-10">
            <div className="my-2">
              <div className="grid lg:grid-cols-4 gap-5">
                <div className="col-span-3">
                  <span className="block text-base font-semibold">
                    Main Customer Account Merge To
                  </span>
                  <Select
                    className="w-full"
                    defaultValue="MR SEUN"
                    options={[
                      {
                        value: "001",
                        label: "001",
                      },
                    ]}
                  />
                </div>
                <div className="col-span-1">
                  <span className="block text-base font-semibold">Branch</span>
                  <Select
                    className="w-full"
                    defaultValue="001"
                    options={[
                      {
                        value: "00059088 |  Abdulazeez",
                        label: "00059088 |  Abdulazeez",
                      },
                    ]}
                  />
                </div>
              </div>
            </div>
            <p className="text-base text-gray-500 mt-3 font-semibold ">
              Customer Search
            </p>
            <div className="grid lg:grid-cols-3 gap-5 my-3">
              <div className="col-span-1">
                <span className="block text-base font-semibold">Surname</span>
                <Input placeholder="NAME" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">CSCS A/C</span>
                <Input placeholder="009" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">CSCS Reg</span>
                <Input placeholder="009" />
              </div>
            </div>

            <div className="grid lg:grid-cols-4 my-5 gap-5">
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Other names
                </span>
                <Input placeholder="009" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  First Name
                </span>
                <Input placeholder="009" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">Names</span>
                <Input placeholder="009" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Subsidiary Account
                </span>
                <Input placeholder="009" />
              </div>
            </div>
            <div className="flex justify-between items-center">
              <Checkbox>Search By Only First Letter</Checkbox>
              <Button className="text-white text-base bg-blue-200">
                Search
              </Button>
            </div>
            <div className=" w-full md:w-4/5 mx-auto  my-10">
          <p className="text-base text-gray-500 my-5 font-semibold">Accounts</p>
          <Table
            size="small"
            className="rounded-xl border"
            scroll={{ x: "max-content" }}
            rowSelection={{
              type: selectionType,
              ...rowSelection,
            }}
            columns={columns}
            dataSource={data}
            bordered
          />
        </div>
        <div className=" w-full md:w-3/5 mx-auto  my-10">
          <p className="text-base text-gray-500 my-5 font-semibold">
            Account To Be Merged
          </p>
          <Table
            size="small"
            className="rounded-xl border"
            scroll={{ x: "max-content" }}
            rowSelection={{
              type: selectionType,
              ...rowSelection2,
            }}
            columns={columns2}
            dataSource={data2}
            bordered
          />
        </div>
          </div>
        </Form>
        {/* <div className=" w-full md:w-4/5 mx-auto  my-10">
          <p className="text-base text-gray-500 my-5 font-semibold">Accounts</p>
          <Table
            size="small"
            className="rounded-xl border"
            scroll={{ x: "max-content" }}
            rowSelection={{
              type: selectionType,
              ...rowSelection,
            }}
            columns={columns}
            dataSource={data}
            bordered
          />
        </div>
        <div className=" w-full md:w-3/5 mx-auto  my-10">
          <p className="text-base text-gray-500 my-5 font-semibold">
            Account To Be Merged
          </p>
          <Table
            size="small"
            className="rounded-xl border"
            scroll={{ x: "max-content" }}
            rowSelection={{
              type: selectionType,
              ...rowSelection2,
            }}
            columns={columns2}
            dataSource={data2}
            bordered
          />
        </div> */}
        <div className="flex justify-center gap-3">
          <Button className="text-red-150 bg-red-250 text-base border-none">
            Delete Merge A/C
          </Button>
          <Button className="text-red-150 bg-red-250 text-base border-none mr-8">
            Remove A/C
          </Button>
          <Button className="text-white text-base bg-blue-200"> Refresh</Button>
          <Button className="text-white text-base bg-blue-200">Merge</Button>
        </div>
      </main>
    </div>
  );
});
