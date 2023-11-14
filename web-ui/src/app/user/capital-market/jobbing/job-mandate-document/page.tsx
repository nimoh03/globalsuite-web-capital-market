"use client";
import { Table, Tooltip, Button, Select, DatePicker } from "antd";
import { images } from "@/theme";
import Image from "next/image";
import type { ColumnsType } from "antd/es/table";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

import { useState } from "react";
import { PaperClipOutlined } from "@ant-design/icons";

interface DataType {
  key: React.Key;
  date: string;
  "document id": string;
  "job order id": string;
  "image file name": string;
  "txn id": string;
  remark: string;
}

const columns: ColumnsType<DataType> = [
  {
    title: <p className="p-0 text-base text-center">Date</p>,
    dataIndex: "date",
    align: "center",
    render: (text) => (
      <p className="font-normal text-sm leading-5">{text}</p>
    )
  },
  {
    title: <p className="p-0 text-base text-center">Document ID</p>,
    dataIndex: "document id",
    align: "center",
    render: (text) => (
      <p className="font-semibold text-sm leading-5" >{text}</p>
    )
  },
  {
    title: <p className="p-0 text-base text-center">Job Order ID</p>,
    dataIndex: "job order id",
    align: "center",
    render: (text) => (
      <p className="font-semibold text-sm leading-5">{text}</p>
    )
  },
  {
    title: <p className="p-0 text-base text-center">Image File Name</p>,
    dataIndex: "image file name",
    align: "center",
    render: (text) => (
      <p className="font-medium text-sm leading-5">{text}</p>
    )
  },
  {
    title: <p className="p-0 text-base text-center">TXN ID</p>,
    dataIndex: "txn id",
    align: "center",
  },
  {
    title: <p className="p-0 text-base text-center">Remark</p>,
    dataIndex: "remark",
    align: "center",
  },
];
const data: DataType[] = [
  {
    key: "1",
    date: "25/04/2022",
    "document id": "PINNEK, EMESHIE LTD -  PINN-8081",
    "job order id": "AHDJD978",

    "image file name": "PMX09812",
    "txn id": "R",
    remark: "Remark",
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

const JobbingMandateDocument = () => {
  const [selectionType, setSelectionType] = useState<"checkbox" | "radio">(
    "checkbox"
  );
  return (
    <div className={manrope.className}>
      <div className="p-5">
        <div className="rounded-lg overflow-hidden shadow-xl">
          <h4 className="bg-blue-300 text-white h-10 flex items-center pl-5 text-base font-extrabold">
            <PaperClipOutlined /> Job Mandate Document Search
          </h4>
          <div className="grid md:grid-cols-5 gap-5 p-3">
            <div>
              <span className="block text-base font-semibold">Customer</span>
              <Select
                defaultValue="JOHNSON"
                options={[
                  {
                    value: "lucy",
                    label: "Lucy",
                  },
                ]}
                style={{ width: "100%" }}
              />
            </div>
            <div>
              <span className="block text-base font-semibold">From</span>
              <DatePicker format="DD/MM/YYYY" />
            </div>
            <div>
              <span className="block text-base font-semibold">To</span>

              <DatePicker format="DD/MM/YYYY" />
            </div>
            <div>
              <span className="block text-base font-semibold">Branch</span>
              <Select
                defaultValue="Lagos"
                options={[
                  {
                    value: "Ibadan",
                    label: "Ibadan",
                  },
                ]}
                style={{ width: "100%" }}
              />
            </div>
            <div>
              <br />
              <Button className="md:ml-10 text-base bg-blue-200 text-white border-none">
                Search
              </Button>
            </div>
          </div>
          <Table
            size="small"
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
      </div>
    </div>
  );
};

export default JobbingMandateDocument;
