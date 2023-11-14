
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
      title: <p className="font-bold text-base leading-6">Security  Name</p>,
      dataIndex: "security name",
      align: "center",
      render: (text) => (
        <p className="font-normal text-base leading-5">{text}</p>
      )
    },
    {
      title: <p className="font-bold text-base leading-6">Security  Code</p>,
      dataIndex: "security code",
      align: "center",
      render: (text) => (
        <p className="font-semibold text-base leading-5">{text}</p>
      )
    },
    {
      title: <p className="font-bold text-base leading-6">Sector</p>,
      dataIndex: "sector",
      align: "center",
      render: (text) => (
        <p className="font-normal text-base leading-5">{text}</p>
      )
    },
    {
      title: <p className="font-bold text-base leading-6">Asset Type</p>,
      dataIndex: "asset type",
      align: "center",
      render: (text) => (
        <p className="font-normal text-base leading-5">{text}</p>
      )
    },
    {
      title: <p className="font-bold text-base leading-6">Registrar</p>,
      dataIndex: "registrar",
      align: "center",
      render: (text) => (
        <p className="font-normal text-base leading-5">{text}</p>
      )
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
            <Link href='/user/capital-market/maintain/securities-assets/security-asset-edit' className="">
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
    'security name': string;
    'security code': string;
    'sector': string;
    'asset type': string;
    'registrar': string;
  }
  const data: DataType[] = [
    {
      key: "1",
      'security name': "Dangote Security",
      'security code': "DAN",
      'sector': "agriculture",
      'asset type': "Bank",
      'registrar': "Bank",
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
          <Link href='/user/capital-market/maintain/securities-assets/security-assest-new'>
          Add New Security 
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
            Security Asset
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
