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
        Job Order
        </h4>
        <div className="p-5">
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
         <div className="grid lg:grid-cols-5 gap-5">
         <div className="my-3 col-span-1">
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
            <div className="my-3 col-span-1">
              <span className="block font-semibold text-base">Transaction Type</span>
              <Select
                defaultValue=""
                className="w-full"
                options={[
                  {
                    value: "All",
                    label: "All",
                  },
                  {
                    value: "Buy Only",
                    label: "Buy Only",
                  },
                  {
                    value: "Sell Only",
                    label: "Sell Only",
                  },
                  {
                    value: "Cross Deal Only",
                    label: "Cross Deal Only",
                  },
                ]}
              />
            </div>
            <div className="lg:w-3/5 mx-auto col-span-3 grid lg:grid-cols-2 items-center">
              <Radio value='by name'>By Name</Radio>
              <Radio value='by name'>By Stock</Radio>
              <Radio value='by name'>Normal</Radio>
              <Radio value='by name'>Detailed</Radio>
              
            </div>
         </div>
         <div className="grid lg:grid-cols-4 gap-5 my-5">
         <div className="my-3 col-span-2">
              <span className="block font-semibold text-base">Broker</span>
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
              <span className="block font-semibold text-base">Prepared BY</span>
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
              <span className="block font-semibold text-base">Checked By</span>
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
       <div className="grid lg:grid-cols-4 gap-5">
        <div className="col-span-2">
            <Checkbox className="my-2">Do Not Show Verified Certificate For Sale</Checkbox>
            <Checkbox className="my-2">Job Order For Date Specified Only</Checkbox>
            <Checkbox className="my-2">Job Order Historical Listing</Checkbox>
        </div>
        <div className="my-3 col-span-1">
              <span className="block font-semibold text-base">Approved By</span>
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
              <span className="block font-semibold text-base">Dealer</span>
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
