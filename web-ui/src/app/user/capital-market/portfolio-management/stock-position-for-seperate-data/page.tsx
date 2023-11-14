"use client";

import { observer } from "mobx-react-lite";
import { DatePicker, Progress, Button } from "antd";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function Page() {
  return (
    <div className={manrope.className}>
      <main className="flex min-h-screen flex-col items-center p-5 gap-16">
        <div
          className="rounded-xl overflow-hidden 
 w-full lg:w-4/5 mx-auto shadow border"
        >
          <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5 mb-5">
            Customer Stock Position Upload- Separate Data
          </h4>
          <div className="w-full md:w-1/4 mx-auto my-5">
            <span className="block text-base mt-5 font-semibold">Effective Date </span>
            <DatePicker format="DD/MM/YYYY" className="w-full" />
          </div>
          <div className="md:w-2/4 mx-auto my-5">
            <Progress percent={50} status="active" />
          </div>
          <div className="w-full md:w-1/4 mx-auto flex justify-around md:justify-between my-5">
            <Button className="bg-red-250 text-white text-base border-none font-bold">
              Cancel
            </Button>
            <Button className="bg-blue-200 text-white border-none font-bold">Run</Button>
          </div>
          <div className="w-full lg:w-3/5 mx-auto">
            <p className="text-base font-bold leading-7 text-gray-500 mt-10 my-3">
              Excel File Column Arrangement
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 1: <span className="text-blue-200">Member Code</span>
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 2: <span className="text-blue-200">CSCS Account</span>
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 3: <span className="text-blue-200">CSCS Account</span>
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 4: <span className="text-blue-200">CSCS CHN NO</span>
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 5: <span className="text-blue-200">Security</span>
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 6: <span className="text-blue-200">Volume Unit</span>
            </p>
            <p className="text-base font-semibold leading-7 text-gray-500">
              Column 7 <span className="text-blue-200">Pending Unit</span>
            </p>
            <span className="text-gray-500 font-bold leading-7 text-base">
              Trade File Name
            </span>
          </div>
        </div>
      </main>
    </div>
  );
});
