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
        Settlement Advice
        </h4>
        <div className="px-5 py-7">
          <div className="grid lg:grid-cols-3 gap-5">
            <div className="col-span-1 ">
              <span className="block text-base font-semibold">From</span>
              <DatePicker format="DD/MM/YYYY" className="w-full" />
            </div>
            <div className="col-span-1 ">
              <span className="block text-base font-semibold">To</span>
              <DatePicker format="DD/MM/YYYY" className="w-full" />
            </div>
            <div className="col-span-1">
              <span className="block font-semibold">Branch</span>
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
        
          <div className="flex justify-between my-5">
            <div className="flex w-1/2 justify-between place-items-end gap-1">
                <div className="w-3/5">
                    <span className="block font-semibold text-base">
                        Market Type
                    </span>
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
                <div>
                    <Checkbox>Net Amount Listing</Checkbox>
                </div>
            </div>
            <div className="flex items-end">
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
