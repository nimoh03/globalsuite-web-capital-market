"use client";
import { DatePicker, Form, Input, Select, Button } from "antd";
import { observer } from "mobx-react-lite";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function Page() {
  return (
    <div className={manrope.className}>
      <main className="min-h-screen m-2">
        <Form className="w-full md:w-4/5 rounded-md overflow-hidden mx-auto shadow bg-gray-50">
          <h4 className="bg-blue-600 p-2 pl-10 text-white text-base">
            Transaction Trade Valuation - Edit
          </h4>
          <div className="p-3">
            <div className="grid grid-cols-3 gap-5">
                <div className="col-span-1">
                    <span className="text-base font-semibold">
                        Transaction no
                    </span>
                    <Input placeholder="0000" />
                </div>
              <div className="col-span-1 ">
                <span className="block text-base font-semibold">Transaction Date</span>
                <DatePicker format="DD/MM/YYYY" className="w-full" />
              </div>
            </div>

            <div className="my-2">
              <div className="lg:grid grid-cols-3 gap-5">
                <div className="col-span-1">
                  <span className="block text-base font-semibold">Product</span>
                  <Select
                    className="w-full"
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
                  <span className="block text-base font-semibold">Subsidiary Account</span>
                  <Select
                    className="w-full"
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
            <div className="lg:grid grid-cols-3 gap-5 my-2">
              <div className="col-span-2">
                <span className="block text-base font-semibold">Stock Code</span>
                <Select
                  className="w-full my-2"
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
                <span className="block text-base font-semibold">Unit Price</span>
                <Form.Item className="my-2">
                  <Input placeholder="9000" />
                </Form.Item>
              </div>
            </div>
            <div className="lg:grid grid-cols-3 gap-10 my-2">
              <div className=" col-span-1">
                <span className="block text-base font-semibold">Quantity</span>
                <Form.Item className="my-2" name="quantity">
                  <Input placeholder="Reference" />
                </Form.Item>
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">Consideration</span>
                <Form.Item className="my-2" name="consideration">
                  <Input placeholder="Reference" />
                </Form.Item>
              </div>
              <div className="col-span-1">
                <span className="block text-base font-semibold">Transaction Type</span>
                <Form.Item className="my-2" name="transaction type">
                  <Input placeholder="Reference" />
                </Form.Item>
              </div>
            </div>
            <div className="lg:grid grid-cols-3 my-2 gap-5">
              <div className=" col-span-1">
                <span className="block text-base font-semibold">Total Fees</span>
                <Form.Item className="my-2" name="Total Fees">
                  <Input placeholder="Reference" />
                </Form.Item>
              </div>
              <div className=" col-span-2">
                <span className="block text-base font-semibold">Total Amount</span>
                <Form.Item className="my-2" name="Total Amount">
                  <Input placeholder="Reference" />
                </Form.Item>
              </div>
            </div>
            <div className="md:grid grid-cols-4 mb-10 my-5 gap-5">
              <div className="col-span-2 my-2">
                <Button
                  htmlType="submit"
                  className="bg-blue-200 text-white border-none font-bold"
                >
                  View Trade Valuation
                </Button>
              </div>
              <div className="col-span-2 my-2">
                <div className="flex md:justify-end gap-7">
                  <Button
                    htmlType="submit"
                    className="bg-red-250 text-red-150 text-base border-none font-bold"
                  >
                    Cancel
                  </Button>
                  <Button className="bg-blue-200 text-white border-none font-bold">
                    Save
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </Form>
      </main>
    </div>
  );
});
