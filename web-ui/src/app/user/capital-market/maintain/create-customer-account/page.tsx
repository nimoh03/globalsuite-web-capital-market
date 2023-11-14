
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
      title: <p className="font-bold text-base leading-6">Date</p>,
      dataIndex: "date",
      align: "center",
      render: (text) => (
        <p className="font-normal text-base leading-5">{text}</p>
      )
    },
    {
      title: <p className="font-bold text-base leading-6">Subsidiary Account</p>,
      dataIndex: "subsidiary account",
      align: "center",
      render: (text) => (
        <p className="font-semibold text-base leading-5">{text}</p>
      )
    },
    {
      title: <p className="font-bold text-base leading-6">TXN ID</p>,
      dataIndex: "txn id",
      align: "center",
    },
    {
      title: <p className="font-bold text-base leading-6">CSCS A/C</p>,
      dataIndex: "cscs a/c",
      align: "center",
    },
    {
      title: <p className="font-bold text-base leading-6">CSCS A/C</p>,
      dataIndex: "cscs a/c",
      align: "center",
    },
    {
      title: <p className="font-bold text-base leading-6">Actions</p>,
      key: "action",
      align:'center',
      responsive: ["sm"],
      width: 150,
      render: () => (
        <div className="flex justify-center items-center gap-4">
         

          <Tooltip placement="bottom" title="edit">
            <Link href='/user/capital-market/maintain/create-customer-account/stockbroking-account-edit' className="">
            <Image
              src={images.pencil_icon}
              alt="icon"
              // className="cursor-pointer w-[15%] text-[10px] "
            />
            </Link>
           </Tooltip>
          <Tooltip placement="bottom" title="delete">
            <Image
              src={images.deleteIcon}
              alt="icon"
              // className="cursor-pointer w-[15%] text-[10px] "
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
    'txn id': string;
    'cscs chn': string;
    "cscs a/c": string;
  }
  const data: DataType[] = [
    {
      key: "1",
      date: "25/04/2022",
      'subsidiary account': "PINNEK, EMESHIE LTD -  PINN-8081",
      'txn id': "PMX09812",
      'cscs chn': "234567890",
      "cscs a/c": "234567890",
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
     <div className="flex justify-between px-5 my-5" >
        <Button className="text-red-150 bg-red-250 text-base border-none">
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
       
        <Button className="text-white text-base bg-blue-200">
          <Link href='/user/capital-market/maintain/create-customer-account/stockbroking-account'>
          Add New Customer Account
          </Link>
         
        </Button>
       </div>
       
      </div>
      <div className="px-3 rounded-lg shadow-xl w-full ">
        <div className={manrope.className}>
          <p className="flex justify-start gap-2 items-center text-base px-5 w-full capitalize bg-[#194bfb] text-white rounded-t-lg h-10">
            <span>
              <Image
                src={images.pin_icon}
                alt="icon"
                className="cursor-pointer  text-[10px] "
              />
            </span>
            StockBroking Customer Account
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
