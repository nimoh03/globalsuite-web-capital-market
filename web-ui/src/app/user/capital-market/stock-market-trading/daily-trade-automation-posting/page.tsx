"use client";

import { observer } from "mobx-react-lite";
import { DatePicker, Button, Radio, Progress } from "antd";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function Page() {
  return (
    <div className={manrope.className}>
      <main className="min-h-screen">
        <div
          className="rounded-lg overflow-hidden 
w-4/5 md:w-4/5 mx-auto shadow-xl mt-5"
        >
          <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5">
            Automated Allotment
          </h4>
          <div className="p-2 mb-5 ">
            <div className="w-full lg:w-1/4 mx-auto">
              <span className="block text-base my-2">Effective Date </span>
              <DatePicker format="DD/MM/YYYY" />
            </div>
            <div className="w-full lg:w-1/4 mx-auto my-3">
              <Radio.Group className=" flex justify-between pl-2">
                <Radio value="floor" className="pl-1">
                  Floor
                </Radio>
                <Radio value="Remote">Remote</Radio>
              </Radio.Group>
            </div>
            <div className="w-full lg:w-2/4 mx-auto">
              <Progress percent={50} status="active" />
            </div>
            <div className="w-full lg:w-1/4 mx-auto flex justify-around md:justify-between my-2">
              <Button className="bg-red-250 text-white text-base border-none">
                Cancel
              </Button>
              <Button className="bg-blue-200 text-white border-none">
                Post
              </Button>
            </div>
            <div className="w-full lg:w-2/4 mx-auto flex flex-col">
              <Button className="block my-5 bg-blue-200 text-white border-none">
                Trades Missing In The Jobbing Book
              </Button>
              <Button className="bg-blue-200 text-white border-none">
                Match Trade With The Jobbing Book
              </Button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
});
