"use client";
import React from "react";
import { DatePicker, Form, Input, Select, Button } from "antd";

import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

function Page() {
  return (
    <main className="min-h-screen">
      <div className={manrope.className}>
        <Form
          name="myForm"
          className="md:w-4/5 rounded-lg overflow-hidden mx-auto border shadow bg-gray-50"
        >
          <h4 className="bg-blue-300 text-white h-10 flex items-center pl-8 text-base font-semibold">
            Job Orders(Mandate) - Edit
          </h4>
          <div className="px-3">
            <div className="grid md:grid-cols-3 gap-5 mt-3">
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Transaction No.
                </span>
                <Input placeholder="0001" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">Date</span>
                <DatePicker format="DD/MM/YYYY" className="shadow-sm w-full" />
              </div>
            </div>

            <div className="my-2">
              <div className="md:grid grid-cols-3 gap-5">
                <div className="col-span-1">
                  <span className="block text-base font-semibold">Product</span>
                  <Select
                    className="w-full shadow-sm"
                    defaultValue="001"
                    options={[
                      {
                        value: "001",
                        label: "001",
                      },
                    ]}
                  />
                </div>
                <div className="col-span-2">
                  <span className="block text-base font-semibold">
                    Subsidiary Account
                  </span>
                  <Select
                    className="w-full shadow-sm"
                    defaultValue="00059088 |  Abdulazeez"
                    options={[
                      {
                        value: "00059088 |  Abdulazeez",
                        label: "00059088 |  Abdulazeez",
                      },
                    ]}
                  />
                </div>
              </div>
              <h5 className="text-green-200 text-xs">
                CSCS A/C: 10113433 | CSCS REG: A909JD334 | Cash Bal: Email:
                abdulazeez@gmail.com | Phone: 08132624118
              </h5>
            </div>

            <div className="md:grid grid-cols-3 gap-5">
              <div className="col-span-2">
                <span className="block text-base font-semibold">
                  Stock Code
                </span>
                <Select
                  className="w-full my-2 shadow-sm"
                  defaultValue="Cash"
                  options={[
                    {
                      value: "00059088 |  Abdulazeez",
                      label: "00059088 |  Abdulazeez",
                    },
                  ]}
                />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Unit Price
                </span>
                <Form.Item className="my-2">
                  <Input placeholder="9000" className="shadow-sm" />
                </Form.Item>
              </div>
            </div>

            <div className="md:grid grid-cols-3 gap-10">
              <div className=" col-span-1">
                <span className="block text-base font-semibold">Quantity</span>
                <Form.Item className="my-2" name="quantity">
                  <Input placeholder="Reference" className="shadow-sm" />
                </Form.Item>
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">Amount</span>
                <Form.Item className="my-2" name="amount">
                  <Input placeholder="Reference" className="shadow-sm" />
                </Form.Item>
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">Ref No.</span>
                <Form.Item className="my-2" name="ref no">
                  <Input placeholder="Reference" className="shadow-sm" />
                </Form.Item>
              </div>
            </div>

            <div className="md:grid grid-cols-3 gap-5 mb-5">
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Transaction type
                </span>
                <Select
                  className="w-full shadow-sm"
                  defaultValue="Buy | Sell | Crossdeal"
                  options={[
                    {
                      value: "00059088 |  Abdulazeez",
                      label: "00059088 |  Abdulazeez",
                    },
                  ]}
                />
              </div>
              <div className="col-span-2">
                <span className="block text-base font-semibold">
                  Cross Deal Selling Customer
                </span>
                <Select
                  className="w-full shadow-sm"
                  defaultValue="Cash"
                  options={[
                    {
                      value: "001",
                      label: "001",
                    },
                  ]}
                />
              </div>
            </div>

            <div className="md:grid grid-cols-3 gap-10">
              <div className=" col-span-1">
                <span className="block text-base font-semibold">
                  Submit Medium
                </span>
                <Select
                  className="w-full my-2 shadow-sm"
                  defaultValue="Sterlying Bank 0201"
                  options={[
                    {
                      value: "001",
                      label: "001",
                    },
                  ]}
                />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                </span>
                <DatePicker format="DD/MM/YYYY" className="shadow-sm w-full" />
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Price Limit
                </span>
                <Form.Item className="my-2">
                  <Input placeholder="Reference" />
                </Form.Item>
              </div>
            </div>

            <div className="md:grid grid-cols-3 gap-10 mt-2 mb-7">
              <div className=" col-span-1">
                <span className="block text-base font-semibold">
                  Pending Order
                </span>
                <Form.Item className="my-2" name="pending order">
                  <Input placeholder="Reference" className="shadow-sm" />
                </Form.Item>
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Due Amount
                </span>
                <Form.Item className="my-2" name="due amount">
                  <Input placeholder="Reference" className="shadow-sm" />
                </Form.Item>
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">
                  Broker To Execute
                </span>
                <Select
                  className="w-full shadow-sm"
                  defaultValue="Sterlying Bank 0201"
                  options={[
                    {
                      value: "001",
                      label: "001",
                    },
                  ]}
                />
              </div>
            </div>

            <div className="md:grid grid-cols-4 mb-5">
              <div className="col-span-2">
                <Button
                  htmlType="submit"
                  className="text-base font-semibold bg-blue-200 text-white border-none"
                >
                  View Unapproved Job
                </Button>
              </div>
              <div className="col-span-2">
                <div className="flex md:justify-end gap-7">
                  <Button
                    htmlType="submit"
                    className="bg-red-250 text-red-150 text-base font-semibold border-none"
                  >
                    Cancel
                  </Button>
                  <Button
                    htmlType="submit"
                    className="text-base font-semibold bg-blue-200 text-white border-none"
                  >
                    Save
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </Form>
      </div>
    </main>
  );
}

export default Page;
