"use client";
import { useState } from "react";
import {
  DatePicker,
  Form,
  Input,
  Select,
  Button,
  Radio,
  Checkbox,
  Modal,
} from "antd";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [value, setValue] = useState();

  return (
    <div className={manrope.className}>
      <main className="flex min-h-screen flex-col items-center gap-16 p-5">
        <Form className="w-full rounded-xl overflow-hidden shadow-2xl bg-gray-50">
          <h4 className="bg-blue-600 p-2 pl-5 text-white text-base">
            StockBroking Customers Statement Of Account
          </h4>
          <div className="px-10 py-3">
            <div className="w-full md:w-2/5 my-3">
              <span className="block text-base font-semibold">Customer</span>
              <Select
                defaultValue="lucy"
                className="w-full"
                options={[
                  {
                    value: "jack",
                    label: "Jack",
                  },
                ]}
              />
            </div>
            <div className="grid lg:grid-cols-3 gap-5 my-3">
              <div className="col-span-1 ">
                <span className="block text-base font-semibold">From</span>
                <DatePicker format="DD/MM/YYYY" className="w-full" />
              </div>
              <div className="col-span-1 ">
                <span className="block text-base font-semibold">To</span>
                <DatePicker format="DD/MM/YYYY" className="w-full" />
              </div>
              <div className="col-span-1">
                <span className="block  text-base font-semibold">Account Type</span>
                <Select
                  defaultValue="NSE"
                  className="w-full"
                  options={[
                    {
                      value: "NSE",
                      label: "NSE",
                    },
                    {
                      value: "Agent",
                      label: "Agent",
                    },
                    {
                      value: "Merge",
                      label: "Merge",
                    },
                    {
                      value: "Investment",
                      label: "Investment",
                    },
                    {
                      value: "NASD",
                      label: "NASD",
                    },
                    {
                      value: "Consolidate",
                      label: "Consolidate",
                    },
                  ]}
                />
              </div>
            </div>

            <Radio.Group value={value} className="flex gap-5 my-3">
              <Radio value="Exclude Reversal">Exclude Reversal</Radio>
              <Radio value="Include Reversal">Include Reversal</Radio>
            </Radio.Group>
            <Radio.Group value={value} className="flex gap-5 my-3">
              <Radio value="NASD">NASD</Radio>
              <Radio value="Consolidated">Consolidated</Radio>
            </Radio.Group>
            <Checkbox className="my-3">Investment Stockbroking A/C</Checkbox>
          </div>
          <div className="flex justify-end my-4 items-center">
            <Button className="bg-blue-200 text-white border-none m-5">
              View
            </Button>
          </div>
        </Form>
      </main>
    </div>
  );
});
