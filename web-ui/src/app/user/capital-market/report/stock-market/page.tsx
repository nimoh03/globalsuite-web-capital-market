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
  <main className="flex min-h-screen flex-col items-center gap-16">
      <Form className="w-full md:w-4/5 rounded-xl overflow-hidden shadow-2xl bg-gray-50">
        <h4 className="bg-blue-600 p-2 pl-5 text-white text-base">
          Contract Note
        </h4>
        <div className="p-3">
          <div className="grid lg:grid-cols-5 gap-5">
            <div className="my-3 col-span-3">
              <span className="block font-semibold text-base">Customer</span>
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
            <div className="my-3 col-span-1">
              <span className="block font-semibold text-base">Security</span>
              <Select
                defaultValue=""
                className="w-full"
                options={[
                  {
                    value: "jack",
                    label: "Jack",
                  },
                ]}
              />
            </div>
            <div className="my-3 col-span-1">
              <span className="block font-semibold text-base">Branch</span>
              <Select
                defaultValue="Lagos"
                className="w-full"
                options={[
                  {
                    value: "jack",
                    label: "Jack",
                  },
                ]}
              />
            </div>
          </div>
          <div className="grid lg:grid-cols-3 gap-5 my-3">
            <div className="col-span-1 ">
              <span className="block text-base font-semibold">From</span>
              <DatePicker format="DD/MM/YYYY" className="w-full" />
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">To</span>
              <DatePicker format="DD/MM/YYYY" className="w-full" />
            </div>
            <div className="col-span-1">
              <span className="block font-semibold text-base">Agent</span>
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
          <div className="grid lg:grid-cols-3 gap-5 my-3">
            <div className="col-span-1">
              <span className="block text-base font-semibold">CSCS A/C</span>
              <Input placeholder="2342324" />
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">CSCS Reg</span>
              <Input placeholder="2342324" />
            </div>
          </div>
          <div className="grid lg:grid-cols-2">
            <div className="col-span-1">
              <Radio.Group  className="flex gap-5 my-3">
                <Radio value="Purchase" className="font-semibold">Purchase</Radio>
                <Radio value="Sale" className="font-semibold">Sale</Radio>
              </Radio.Group>
              <Radio.Group className="flex gap-5 my-3">
                <Radio value="Equity">Equity</Radio>
                <Radio value="NASD">N.A.S.D</Radio>
                <Radio value="Bond">Bond</Radio>
              </Radio.Group>
              <Radio.Group className="flex gap-5 my-3">
                <Radio value="Landscape">Landscape</Radio>
                <Radio value="Portrait">Portrait</Radio>
              </Radio.Group>
            </div>
            <div className="col-span-1">
              <Radio.Group  className="flex gap-5 my-2 flex-col">
                <Radio value=" Batch">Batch</Radio>
                <Radio value="Single">Single</Radio>
                <Radio value="Group batch">Group Batch</Radio>
              </Radio.Group>
            </div>
          </div>
        </div>
        <div className="flex justify-end items-end">
          <Button className="bg-blue-200 text-white border-none m-5">
            View
          </Button>
        </div>
      </Form>
    </main>
    </div>
  
  );
});
