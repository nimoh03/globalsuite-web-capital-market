"use client";
import React from "react";
import { Descriptions, Button } from "antd";
import {
  TagOutlined,
  PaperClipOutlined,
  DollarOutlined,
} from "@ant-design/icons";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

const items = [
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        <span>Transaction No</span>
      </div>
    ),
    children: "00192NCJA",
  },
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Date
      </div>
    ),
    children: "Prepaid",
  },
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Product
      </div>
    ),
    children: "18:00:00",
  },
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Subsidiary A/C
      </div>
    ),
    children: "18:00:00",
  },

  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Stock Code
      </div>
    ),
    children: "18:00:00",
  },
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Actual Unit Cost
      </div>
    ),
    children: "18:00:00",
  },
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Quantity
      </div>
    ),
    children: "18:00:00",
  },
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl" />
        Total Amount
      </div>
    ),
    children: "18:00:00",
  },
];
function Page() {
  return (
    <>
      <div className={manrope.className} style={{ overflow: "auto" }}>
        <div className="text-center my-5">
          <h1 className="text-4xl">120,000.00</h1>
          <h5 className="text-sm">
            Added Stock Portfolio Holding Balance By: Ahmad Rufai
          </h5>
          <h5 className="text-sm">For:GTB</h5>
        </div>
        <div className="w-full mx-auto">
          <div className="rounded-xl overflow-hidden border shadow ">
            <h4 className="bg-blue-300 text-white h-10 flex items-center pl-5 text-base">
              <PaperClipOutlined />
              Added Stock Portfolio Holding Detail
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
                size="small"
                contentStyle={{ backgroundColor: "#E8EDFF", width: "30%" }}
                labelStyle={{ backgroundColor: "00#D8E3F8" }}
              />
              <h5 className="text-green-200 text-xs my-3">
                CSCS A/C: 10113433 | CSCS REG: A909JD334 | Cash Bal: Email:
                abdulazeez@gmail.com | Phone: 08132624118
              </h5>
              <div className="flex flex-col lg:flex-row justify-between">
                <div className=" flex gap-5">
                  <Button
                    htmlType="submit"
                    className="text-base bg-gray-100 text-blue-200 border-none font-bold"
                  >
                    View Unapproved Add To Portfolio Holding
                  </Button>
                  <Button
                    htmlType="submit"
                    className="text-base bg-gray-100 text-blue-200 border-none font-bold"
                  >
                    Add New Holding
                  </Button>
                </div>
                <div className="col-span-1">
                  <div>
                    <Button
                      htmlType="submit"
                      className="text-base bg-blue-200 text-white border-none font-bold"
                    >
                      Approve
                    </Button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div className="flex justify-between my-5 px-2">
            <span>
              <DollarOutlined /> Status
            </span>
            <Button className="bg-red-250 text-red-150 border-bone">
              Unapproved
            </Button>
          </div>
        </div>
      </div>
    </>
  );
}

export default Page;
