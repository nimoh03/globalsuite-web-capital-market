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
        Trade Summary
        </h4>
        <div className="p-5"> 
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
         <div className="grid lg:grid-cols-5 gap-5 my-2">
         <div className="col-span-5">
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
         <div className="grid lg:grid-cols-2 gap-5 my-2">
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
           <span className="block font-semibold text-base">Report by</span>
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
       <div className="grid lg:grid-cols-2 gap-5 my-2">
        <div className=" col-span-1">
              <span className="block font-semibold text-base">Transaction Type</span>
              <Select
                defaultValue=""
                className="w-full"
                options={[
                  {
                    value: "Purchase",
                    label: "Purchase",
                  },
                  {
                    value: "Sale",
                    label: "Sale",
                  },
                  {
                    value: "All Trades Type",
                    label: "All Trades Type",
                  },
                ]}
              />
            </div>
        <div className="col-span-1">
              <span className="block font-semibold text-base">Transaction Type</span>
              <Select
                defaultValue=""
                className="w-full"
                options={[
                  {
                    value: "Posted",
                    label: "Posted",
                  },
                  {
                    value: "All (No Reversal)",
                    label: "All (No Reversal)",
                  },
                  {
                    value: "Unposted",
                    label: "Unposted",
                  },
                  {
                    value: "Reverse",
                    label: "Reverse",
                  },
                  {
                    value: "All",
                    label: "All",
                  },
                ]}
              />
            </div>
           
       </div>
       <div className="grid lg:grid-cols-2 items-center mt-2">
                <div className="col-span-1">
                <Radio value='All'>All</Radio>
                <Radio value='Creditor only'>Creditor Only</Radio>
                <Radio value='Debtor only' >Debtor Only</Radio>
                </div>
                <div className="col-span-1">
                    <span className="block font-semibold text-base">
                        Market Type
                    </span>
              <Select
                defaultValue="Lagos"
                className="w-3/4"
                options={[
                  {
                    value: "jack",
                    label: "Jack",
                  },
                ]}
              />
                </div>
                <Checkbox className="col-span-1">Include Customer Balance</Checkbox>
       </div>
        </div>
        <div className="flex justify-Between px-5">
            <div className="flex gap-3">
                <Checkbox className="w-full place-self-center">Generate Minor Customer Only</Checkbox>
                <Input placeholder="2342324" className="w-2/5 place-self-center"/>
            </div>
            <div className="ms-auto">
            <Button className="bg-blue-200 text-white border-none m-5">
            View
          </Button>
            </div>
         
        </div>
      </Form>
    </main>
    </div>
   
  );
});
