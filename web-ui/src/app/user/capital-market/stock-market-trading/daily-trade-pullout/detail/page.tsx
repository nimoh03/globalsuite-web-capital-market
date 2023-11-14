"use client";

import { observer } from "mobx-react-lite";
import { Descriptions, Progress, Button } from "antd";
import { TagOutlined } from "@ant-design/icons";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function Page() {
  const items = [
    {
      label: (
        <div className="flex items-center gap-3">
          <TagOutlined className="-rotate-90 text-xl"/>
          <span className="text-base font-medium">Posted Date</span>
        </div>
      ),
      children: <h2 className="text-base font-bold">30/10/2023</h2>,
    },
  ];
  return (
    <div className={manrope.className}>
      <main className="min-h-screen">
        <div className="rounded-lg overflow-hidden w-full md:w-4/5 mx-auto border shadow m-2">
          <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5">
            Daily Trade Pull Out / Reverse
          </h4>
          <div className="py-10 px-2">
            <div className="md:p-10 w-4/5 mx-auto">
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
                size='small'
                contentStyle={{backgroundColor:'#E8EDFF',width:'60%'}}
                labelStyle={{backgroundColor:'00#D8E3F8'}}
              />
            </div>
            <div className="md:w-2/4 mx-auto">
              <Progress percent={50} status="active" />
            </div>
            <div className="md:w-2/4 mx-auto flex justify-around my-5 gap-5">
              <Button className="bg-blue-200 text-white border-none font-bold">View Approved Trade Upload</Button>
              <Button className="bg-blue-200 text-white border-none font-bold">Reverse</Button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
});
