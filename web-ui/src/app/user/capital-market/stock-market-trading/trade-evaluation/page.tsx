
"use client";
import { observer } from "mobx-react-lite";
import Link from "next/link";
import { useState } from "react";
import { Table, Tooltip , Button , Select} from "antd";
import { images } from "@/theme";
import Image from "next/image";
import type { ColumnsType } from "antd/es/table";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function AccountingCustomerDepositCreditNote() {
  const [selectionType, setSelectionType] = useState<"checkbox" | "radio">(
    "checkbox"
  );
  const columns: ColumnsType<DataType> = [
    {
      title: <p className="p-0 text-base text-center font-bold ">Date</p>,
      dataIndex: "date",
      align: "center",
      width: 80,
      responsive: ["sm"],
      render: (text) => (
        <p className="font-normal text-sm leading-5">{text}</p>
      )
    },
    {
      title: <p className="p-0 text-base text-center font-bold">Subsidiary Account</p>,
      dataIndex: "subsidiary account",
      align: "center",
      width: 250,
      // responsive: ["sm"],
      render: (text) => (
        <p className="font-semibold text-sm leading-5" >{text}</p>
      )
    },
    {
      title: <p className="p-0 text-base text-center font-bold">Security</p>,
      dataIndex: "security",
      align: "center",
      width: 80,
      responsive: ["sm"],
      render: (text) => (
        <p className="font-semibold text-sm leading-5">{text}</p>
      )
    },
    {
      title: <p className="p-0 text-base text-center font-bold">Quantity</p>,
      dataIndex: "quantity",
      align: "center",
      width: 80,
      responsive: ["sm"],
      render: (text) => (
        <p className="font-medium text-sm leading-5">{text}</p>
      )
    },
    {
      title: <p className="p-0 text-base text-center font-bold">Price</p>,
      dataIndex: "price",
      align: "center",
      width: 80,
      responsive: ["sm"],
      render: (text) => (
        <p className="font-medium text-sm leading-5">{text}</p>
      )
    },
    {
      title: <p className="p-0 text-base text-center font-bold">TXN ID</p>,
      dataIndex: "txn id",
      align: "center",
      width: 80,
      responsive: ["sm"],
      render: (text) => (
        <p className="font-medium text-sm leading-5">{text}</p>
      )
    },
    {
      title: <p className="p-0 text-base text-center">Actions</p>,
      key: "action",
      responsive: ["sm"],
      width: 150,
      render: () => (
        <div className="flex justify-center items-center gap-4">
          <Tooltip placement="bottom" title="approve">
            <Image
              src={images.checkIncon}
              alt="icon"
            />
          </Tooltip>

          <Tooltip placement="bottom" title="edit">
            <Link href='//user/capital-market/stock-market-trading/trade-evaluation/edit'>
            <Image
              src={images.pencil_icon}
              alt="icon"
            />
            </Link>
           </Tooltip>
          <Tooltip placement="bottom" title="delete">
            <Image
              src={images.deleteIcon}
              alt="icon"
            />
          </Tooltip>
        </div>
      ),
    },
  ];
  interface DataType {
    key: React.Key;
    date: string;
    'subsidiary account': string;
    security: string;
    quantity: string;
    "price": string;
    "txn id": string;
  }
  const data: DataType[] = [
    {
      key: "1",
      date: "25/04/2022",
      'subsidiary account': "PINNEK, EMESHIE LTD -  PINN-8081",
      security: "₦10,000,000.00",
      quantity: "PMX09812",
      "price": "R",
      "txn id": "R",
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
  return (
    <>
     <div className=" flex overflow-auto gap-3 lg:justify-between lg:gap-7 my-3 px-3" >
        <Button className="text-red-150 bg-red-250 text-base border-none font-bold">
          Batch Delete
        </Button>
       <div className="flex gap-5">
       <Select
        className="w-36"
          showSearch
          placeholder="Filter"
          optionFilterProp="children"
          options={[
            {
              value: "user",
              label: "User",
            },
          ]}
        />
        <Button className="text-white text-base bg-blue-200 font-bold">
          <Link href='/user/capital-market/stock-market-trading/trade-evaluation/new'>
          Add New Trade Valuation
          </Link>
         
        </Button>
       </div>
        
      </div>
      <div className="mx-3 rounded-lg shadow-xl w-full ">
        <div className={manrope.className}>
          <p className="flex justify-start gap-2 items-center text-base px-5 w-full  bg-[#194bfb] text-white rounded-t-lg h-10">
            <span>
              <Image
                src={images.pin_icon}
                alt="icon"
                className="cursor-pointer  text-[10px] "
              />
            </span>
            Trade Valuation
          </p>
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
        {/* </div> */}
      </div>
    </>

  );
});
