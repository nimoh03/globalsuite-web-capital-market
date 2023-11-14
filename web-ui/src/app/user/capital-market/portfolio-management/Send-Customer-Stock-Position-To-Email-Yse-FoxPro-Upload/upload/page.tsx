"use client";

import { observer } from "mobx-react-lite";
import { DatePicker, Button, Radio, Progress, Input, Descriptions } from "antd";
import { TagOutlined } from "@ant-design/icons";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

const items = [
  {
    label: (
      <div className="flex items-center gap-5">
        <TagOutlined className="-rotate-90 text-xl"/>
        <span>Upload Date</span>
      </div>
    ),
    children: <p className="font-bold text-base">30/10/2023</p>,
  },
];
export default observer(function Page() {
  return (
    <div className={manrope.className}>
      <main className="min-h-screen p t-20">
        <div
          className="rounded-lg overflow-hidden 
 w-full md:w-3/5 mx-auto shadow-xl "
        >
          <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5 justify-center">
            Send Customer Stock Position To Email - Use FoxPro Upload
          </h4>
          <div className="my-5">
            <div className="w-full lg:w-4/5 mx-auto">
              <span className="block font-semibold text-base">
                Transaction No.
              </span>
              <Input placeholder="Basic usage" className="w-full" />
            </div>
            <div className="md:p-10 w-full lg:w-4/5 mx-auto">
              <Descriptions
                bordered
                column={{
                  xs: 2,
                  sm: 2,
                  md: 2,
                  lg: 2,
                  xl: 2,
                  xxl: 2,
                }}
                items={items}
                size="small"
                contentStyle={{ backgroundColor: "#E8EDFF", width: "65%" }}
                labelStyle={{ backgroundColor: "00#D8E3F8" }}
              />
            </div>
            <div className="w-full md:w-1/4 mx-auto flex justify-around md:justify-center gap-3 my-2">
              <Button className="bg-red-250 text-white text-base border-none">
                Cancel
              </Button>
              <Button className="bg-blue-200 text-white border-none">
                Send
              </Button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
});
