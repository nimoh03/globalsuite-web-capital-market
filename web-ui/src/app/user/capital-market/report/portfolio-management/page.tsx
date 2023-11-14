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
  <main className="flex min-h-screen flex-col items-center gap-16 bg-gray-50">
      <Form className="w-full md:w-4/5 rounded-xl overflow-hidden shadow-2xl ">
        <h4 className="bg-blue-600 p-2 pl-5 text-white text-base">
        Portfolio Holding
        </h4>
        <div className="px-5 py-5">
          <div className="grid lg:grid-cols-2 gap-5">
            <div className="col-span-1 ">
              <span className="block text-base font-semibold">Date As At</span>
              <DatePicker format="DD/MM/YYYY" className="w-full" />
            </div>
            <div className="col-span-1">
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
          <div className="my-3 font-semibold">Print All Investment Accounts <Checkbox></Checkbox></div>
           <div className="grid lg:grid-cols-2 gap-5">
           <div className="col-span-2">
              <span className="block font-semibold text-base">Customer</span>
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
           </div>
           <div className="grid lg:grid-cols-2 gap-5 my-3">
           <div className="col-span-1">
              <span className="block font-semibold text-base">Stock Code</span>
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
            <div className="col-span-1">
              <span className="block font-semibold text-base">To Be Determined</span>
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
           </div>
          <div className="flex justify-between items-end">
            <div className="flex flex-col">
                <Checkbox className="my-1">Order By Security Code</Checkbox>
                <Checkbox>Do Not Show Cost</Checkbox>
            </div>
            <div>
            <Button className="bg-blue-200 text-white border-none">
              View
            </Button>
            </div>
            
          </div>
        </div>
      </Form>
    </main>
    </div>
  
  );
});
