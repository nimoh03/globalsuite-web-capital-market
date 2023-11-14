"use client";
import React from "react";
import { Descriptions, Button } from "antd";
import { TagOutlined, PaperClipOutlined, DollarOutlined } from "@ant-design/icons";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

const items = [
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        <span className="text-base font-medium">Transaction No</span>
      </div>
    ),
    children: <p className="text-base font-bold">00192NCJA</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Date
      </div>
    ),
    children: <p className="text-base font-bold">Prepaid</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Product
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Subsidiary A/C
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Stock Code
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Unit Price
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Quantity
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Amount
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Reference No.
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Trans. Type
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Submit Medium
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Date Limit
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Price Limit
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Pending order
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Due Amount
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl"/> 
        Broker to Exe.
      </div>
    ),
    children:  <p className="text-base font-bold">18:00:00</p>,
  },
];
function Page() {
  return (
    <div className={manrope.className}>
      <div className="overflow-auto w-full">
      <div className="text-center mt-3 w-full">
        <h1 className="text-4xl">120,000.00</h1>
        <h5 className="text-sm">job order By: Ahmad Rufai</h5>
        <h5 className="text-sm">For : GTB</h5>
      </div>
      <div className="sm:p-10 w-full">
        <div className="rounded-xl overflow-hidden border shadow w-full">
          <h4 className="bg-blue-300 text-white h-10 flex items-center gap-3 pl-5 text-base">
            <PaperClipOutlined /> Job Order Detail
          </h4>
          <div className="p-2 w-full">
            <Descriptions
              bordered
              column={{
                xs: 2,
                sm: 2,
                md: 2,
                lg: 2,
                xl: 2,
                xxl: 2,
              }}
              items={items}
              className="w-full"
              size='small'
              contentStyle={{backgroundColor:'#E8EDFF',width:'30%'}}
              labelStyle={{backgroundColor:'00#D8E3F8'}}
            />
            <h5 className="text-green-200 text-xs my-3">
              CSCS A/C: 10113433 | CSCS REG: A909JD334 | Cash Bal: Email:
              abdulazeez@gmail.com | Phone: 08132624118
            </h5>
            <div className="grid sm:grid-cols-4 my-5 items-center">
              <div className="col-span-3 flex gap-5">
                <Button
                  htmlType="submit"
                  className="text-base bg-gray-100 text-blue-200 border-none font-bold"
                >
                  View Unapproved Job Order
                </Button>
                <Button
                  htmlType="submit"
                  className="text-base bg-gray-100 text-blue-200 border-none font-bold"
                >
                  Add New Job Order
                </Button>
              </div>
              <div className="col-span-1 ms-auto">
                <div>
                  <Button
                    htmlType="submit"
                    className="text-base bg-blue-200 text-white border-none font-bold sm:my-2"
                  >
                    Approve
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="flex justify-between px-10 my-5">
          <span><DollarOutlined /> Status</span>
          <Button className="text-orange-100 border-none bg-red-250">Unapproved</Button>
      </div>
      </div>
     
    </div>
  );
}

export default Page;
