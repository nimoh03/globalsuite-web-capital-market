"use client";

import { observer } from "mobx-react-lite";
import { DatePicker, Input, Select, Checkbox, Button } from "antd";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });


export default observer(function Page() {
  return (
    <div className={manrope.className}>
 <main className="flex min-h-screen flex-col items-center border-t-0 shadow w-4/5 mx-auto my-5">
      <h4 className="flex justify-start gap-2 items-center text-base px-10 w-full capitalize bg-[#194bfb] text-white rounded-t-xl h-10">
        Customer StockBroking Account - New
      </h4>
      <div className="w-full bg-gray-50 px-5">
        <div className="grid lg:grid-cols-3 gap-5">
          <div className="col-span-1">
            <span className="block text-base font-semibold my-2">Transaction Date </span>
            <DatePicker format="DD/MM/YYYY" className="w-full" />
          </div>
          <div className="col-span-2">
            <span className="block text-base font-semibold my-2">Branch A/C </span>
            <Select
              defaultValue="Lagos"
              className="w-full"
              options={[
                {
                  value: "jack",
                  label: "Jack",
                },
                {
                  value: "lucy",
                  label: "Lucy",
                },
                {
                  value: "Yiminghe",
                  label: "yiminghe",
                },
                {
                  value: "disabled",
                  label: "Disabled",
                  disabled: true,
                },
              ]}
            />
          </div>
        </div>
        <div className="grid lg:grid-cols-3 gap-5">
          <div className="col-span-2">
            <span className="block text-base font-semibold my-2">Subsidiary Account </span>
            <Select
              defaultValue="Lagos"
              className="w-full"
              options={[
                {
                  value: "jack",
                  label: "Jack",
                },
                {
                  value: "lucy",
                  label: "Lucy",
                },
                {
                  value: "Yiminghe",
                  label: "yiminghe",
                },
                {
                  value: "disabled",
                  label: "Disabled",
                  disabled: true,
                },
              ]}
            />
          </div>
          <div className="col-span-1 text-base ">
            <span className="block my-2">CSCS CHN #</span>
            <Input placeholder="001" />
          </div>
        </div>
        <h5 className="text-green-200 text-xs my-3">
          Email: abdulazeez@gmail.com | Phone: 08132624118
        </h5>
        <div className="grid lg:grid-cols-3 gap-5">
          <div className="col-span-1">
            <span className="block text-base font-semibold">CSCS A/C</span>
            <Input placeholder="90000" />
          </div>
          <div className="col-span-1">
            <span className="block text-base font-semibold">Buy Comm</span>
            <Input placeholder="Cash" />
          </div>
          <div className="col-span-1">
            <span className="block text-base font-semibold">Sale Comm</span>
            <Input placeholder="Cash" />
          </div>
        </div>
        <div className="my-5 flex gap-5">
          <Checkbox>Sub Dealer A/C</Checkbox>
          <Checkbox>Proprietary A/C</Checkbox>
        </div>
        <div className="grid lg:grid-cols-3 gap-5">
          <div className="col-span-1">
            <span className="block text-base font-semibold">Sub Dealer </span>
            <Select
              defaultValue="SOMETHING"
              className="w-full"
              options={[
                {
                  value: "jack",
                  label: "Jack",
                },
                {
                  value: "lucy",
                  label: "Lucy",
                },
                {
                  value: "Yiminghe",
                  label: "yiminghe",
                },
                {
                  value: "disabled",
                  label: "Disabled",
                  disabled: true,
                },
              ]}
            />
          </div>
          <div className="col-span-1">
            <span className="block font-semibold text-base">Sub Dealer Comm</span>
            <Input placeholder="NOTHING" />
          </div>
          <div className="col-span-1">
            <span className="block text-base font-semibold">DCS Setup Date</span>
            <DatePicker format="DD/MM/YYYY" className="w-full" />
          </div>
        </div>
        <div className="my-5 flex gap-5">
          <Checkbox>Direct Cash Settlement- NGX</Checkbox>
          <Checkbox>Do Not Charge Stamp Duty</Checkbox>
        </div>
        <div className="my-5 flex gap-5">
          <Checkbox>NASD A/C</Checkbox>
          <p>Status : Deactivate <Checkbox></Checkbox></p>
        </div>
        <div className="my-5">
            <Button className="text-base bg-blue-200 text-white border-none">Generate CSCS A/C Opening File</Button>
        </div>
        <div className="flex justify-between items-center my-5">
        <Button className="text-base bg-blue-200 text-white border-none">Update CSCS A/C From CSCS Website</Button>
        <div className="flex gap-5">
        <Button className="text-base bg-red-200 text-red-150 border-none font-semibold">Cancel</Button>
        <Button className="text-base bg-blue-200 text-white border-none font-semibold">Save</Button>
        </div>
        </div>
       
      </div>
    </main>
    </div>
   
  );
});
