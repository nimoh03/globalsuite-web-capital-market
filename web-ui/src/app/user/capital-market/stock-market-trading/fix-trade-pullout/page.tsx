"use client";

import { observer } from "mobx-react-lite";
import { DatePicker, Progress, Button } from "antd";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function Page() {
  return (
    <div className={manrope.className}>
      <main className="min-h-screen">
        <div className="md:w-3/5 mx-auto rounded-lg border shadow overflow-hidden bg-gray-50">
          <div>
            <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5">
              Fix Trade Pull Out- Reversal
            </h4>
          </div>
          <div className="p-2">
            <h3 className="w-full text-base lg:w-2/5 mx-auto my-3 font-bold">
              Reverse Fix For All Three Dates
            </h3>
            <div className="w-full lg:w-2/5 mx-auto my-5">
              <span className="block text-base my-5 font-semibold">Trade Summary Date </span>
              <DatePicker format="DD/MM/YYYY" className="w-full"/>
              <span className="block text-base my-5 font-semibold">
                Account Statement Date
              </span>
              <DatePicker format="DD/MM/YYYY" className="w-full"/>
              <span className="block text-base my-5 font-semibold">Portfolio Date </span>
              <DatePicker format="DD/MM/YYYY" className="w-full"/>
            </div>
            <div className="lg:w-2/4 mx-auto my-5">
              <Progress percent={50} status="active" />
            </div>
            <div className="lg:w-2/4 mx-auto flex justify-center gap-5 my-5">
              <Button className="bg-red-250 text-white text-base border-none font-bold">Cancel</Button>
              <Button className="bg-blue-200 text-white border-none font-bold">Pull Out</Button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
});
