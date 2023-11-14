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
        Statutory Return
        </h4>
        <div className="p-3">
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
         <div className="grid lg:grid-cols-5 gap-5">
         <div className="my-2 col-span-5">
              <span className="block font-semibold">Customer</span>
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
         <div className="grid lg:grid-cols-3 gap-5 my-2">
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
              <span className="block font-semibold text-base">EFCC Reports</span>
              <Select
                defaultValue=""
                className="w-full"
                options={[
                  {
                    value: "Major Deals-Daily",
                    label: "Major Deals-Daily",
                  },
                  {
                    value: "Major Deals-Weekly",
                    label: "Major Deals-Weekly",
                  },
                  {
                    value: "Disclosure Of Transaction By Multiples Transaction By Unit Price",
                    label: "Disclosure Of Transaction By Multiples Transaction By Unit Price",
                  },
                  {
                    value: "Disclosure Of Transaction By Multiples Transaction By Stock code",
                    label: "Disclosure Of Transaction By Multiples Transaction By Stock code",
                  },
                  {
                    value: "Money Laundering Report 1",
                    label: "Money Laundering Report 1",
                  },
                  {
                    value: "Money Laundering Report 2",
                    label: "Money Laundering Report 2",
                  },
                  {
                    value: "Foreign Investment Portfolio",
                    label: "Foreign Investment Portfolio",
                  },
                  {
                    value: "Foreign Exchange Transaction Returns",
                    label: "Foreign Exchange Transaction Returns",
                  },
                  {
                    value: "Suspicious Transactions",
                    label: "Suspicious Transactions",
                  },
                  {
                    value: "Stockbroking Transaction Report(Monthly)",
                    label: "Stockbroking Transaction Report(Monthly)",
                  },
                ]}
              />
            </div>
            <div className="col-span-1">
              <span className="block font-semibold text-base">Other Regulatory Returns</span>
              <Select
                defaultValue=""
                className="w-full"
                options={[
                  {
                    value: "C.S.C.S Fee Returns",
                    label: "C.S.C.S Fee Returns",
                  },
                  {
                    value: "N.S.E Fee Returns",
                    label: "N.S.E Fee Returns",
                  },
                  {
                    value: "V.A.T Fee",
                    label: "V.A.T Fee",
                  },
                  {
                    value: "V.A.T Fee Name",
                    label: "V.A.T Fee Name",
                  },
                  {
                    value: "S.E.C Fee Returns",
                    label: "S.E.C Fee Returns",
                  },
                  {
                    value: "Contract Stamp Returns",
                    label: "Contract Stamp Returns",
                  },
                  {
                    value: "Contract Stamp-Details",
                    label: "Contract Stamp-Details",
                  },
                ]}
              />
            </div>
         </div>
       <div className="grid lg:grid-cols-3 gap-5 ">
        <div className="col-span-1">
            <span className="block font-semibold text-base">Amount</span>
            <Input placeholder="2342324" />
        </div>
        <div className=" col-span-1">
              <span className="block font-semibold text-base">Market Type</span>
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
            <div className="col-span-1 mt-8">
             <Checkbox>Exclude Proprietary A/C</Checkbox>
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
