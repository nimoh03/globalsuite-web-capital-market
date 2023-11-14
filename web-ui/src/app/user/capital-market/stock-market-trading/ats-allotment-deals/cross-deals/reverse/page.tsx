"use client";
import React from "react";
import { Descriptions, Button, Form, Checkbox } from "antd";
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
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        <span className="text-base font-medium">Transaction No</span>
      </div>
    ),
    children: <p className="text-base font-bold">00192NCJA</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Date
      </div>
    ),
    children: <p className="text-base font-bold">Prepaid</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Ticket
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Branch
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Product
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
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
        <TagOutlined className="-rotate-90 text-xl" />
        Quantity
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Price
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Consideration
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Sold By
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        Bought By
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        No Of Trans
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
];

const items2 = [
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        SEC
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        COMM VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        STAMP
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        NSE
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        COMMISION
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        CSCS
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        SEC VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        NSE VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        CSCS VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        SMS ALERT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        ALERT ALERT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        TOTAL
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
];

const items3 = [
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        SEC
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3">
        <TagOutlined className="-rotate-90 text-xl" />
        COMM VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        STAMP
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        NSE
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        COMMISION
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        CSCS
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        SEC VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        NSE VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        CSCS VAT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        SMS ALERT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        ALERT ALERT
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
  {
    label: (
      <div className="flex items-center gap-3 ">
        <TagOutlined className="-rotate-90 text-xl" />
        TOTAL
      </div>
    ),
    children: <p className="text-base font-bold">18:00:00</p>,
  },
];

function Page() {
  return (
    <div className={manrope.className} style={{ overflow: "auto" }}>
      <div className="text-center mt-4">
        <h1 className="text-4xl">120,000.00</h1>
        <h5 className="text-sm">Cross Deal By: Ahmad Rufai</h5>
        <h5 className="text-sm">For:GTB</h5>
      </div>
      <div className="w-full rounded-xl overflow-hidden border shadow">
        <div className="">
          <h4 className="bg-blue-300 text-white h-10 flex items-center pl-5 text-base">
            <PaperClipOutlined /> Cross Deals Detail
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
          </div>
        </div>
        <div className="mt-5">
          <h4 className="bg-blue-300 text-white h-10 flex text-base items-center justify-center rounded-t-xl">
            <PaperClipOutlined /> Buyer Charges & Gross Amount
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
              items={items2}
              size="small"
              contentStyle={{ backgroundColor: "#E8EDFF", width: "30%" }}
              labelStyle={{ backgroundColor: "00#D8E3F8" }}
            />
          </div>
        </div>
        <div className="mt-5">
          <h4 className="bg-blue-300 text-white h-10 flex items-center justify-center text-base rounded-t-xl">
            <PaperClipOutlined /> Seller Charges & Net Amount
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
              items={items3}
              size="small"
              contentStyle={{ backgroundColor: "#E8EDFF", width: "30%" }}
              labelStyle={{ backgroundColor: "00#D8E3F8" }}
            />
            <div className="my-4">
              <h5 className="text-blue-200 text-medium font-semibold">
                Buyer Details
              </h5>
              <div className="grid md:grid-cols-5 gap-2 items-center">
                <div className="col-span-3">
                  <h5 className="text-green-200 text-xs">
                    CSCS A/C: 10113433 | CSCS REG: A909JD334 | Cash Bal: Email:
                    abdulazeez@gmail.com | Phone: 08132624118
                  </h5>
                </div>
                <div className="col-span-2 flex gap-5 ">
                  <Checkbox className="mr-5">Auto Post</Checkbox>

                  <Checkbox>Unsaved Reversal</Checkbox>
                </div>
              </div>
              <h5 className="text-blue-200 text-medium font-semibold">
                Seller Details
              </h5>
              <h5 className="text-green-200 text-xs">
                CSCS A/C: 10113433 | CSCS REG: A909JD334 | Cash Bal: Email:
                abdulazeez@gmail.com | Phone: 08132624118
              </h5>
            </div>
            <div className="flex flex-col justify-between lg:flex-row">
              <div className="flex gap-5 my-1">
                <Button
                  htmlType="submit"
                  className="text-base bg-gray-100 text-blue-200 border-none font-bold"
                >
                  View Unapproved Cross Deals
                </Button>
                <Button
                  htmlType="submit"
                  className="text-base bg-gray-100 text-blue-200 border-none font-bold"
                >
                  Add New Deals
                </Button>
              </div>
              <div className="my-1">
                <div>
                  <Button
                    htmlType="submit"
                    className="text-base bg-blue-200 text-white border-none"
                  >
                    Reverse
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="flex justify-between px-10 my-5">
        <span>
          <DollarOutlined /> Status
        </span>
        <Button className="text-green-200 border-none bg-red-250">
          Unapproved
        </Button>
      </div>
    </div>
  );
}

export default Page;
