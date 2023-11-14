"use client";

import { observer } from "mobx-react-lite";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Input, Button, Select, Checkbox } from "antd";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

export default observer(function Page() {
  const pathname = usePathname();
  return (
    <div className={manrope.className}>
      <main className="flex min-h-screen flex-col items-center p-5 gap-16">
        <div className="border rounded-xl shadow w-full bg-gray-50">
          <h4 className="flex justify-start items-center text-base px-10 w-full capitalize bg-[#194bfb] text-white rounded-t-xl h-10">
          StockBroking Firm - New
          </h4>
          <div className="p-5">
          <div className="grid lg:grid-cols-3 gap-5 my-3">
            <div className="col-span-1">
                <span className="block text-base font-semibold">
                StockBroker Code
                </span>
                <Input placeholder="Reference" />
            </div>
            <div className="col-span-2">
                <span className="block text-base font-semibold">
                StockBroker Name
                </span>
                <Input placeholder="Reference" />
            </div>
        
        </div>
        <div className="grid lg:grid-cols-2 gap-5 my-5">
            <div className="col-span-2">
                <span className="block text-base font-semibold">Address</span>
                <Input placeholder="Reference" />
            </div>
        </div>
        <div className="grid lg:grid-cols-3 gap-5 my-5">
            <div className="col-span-1">
                <span className="block text-base font-semibold">
                    Telephone
                </span>
                <Input placeholder="Reference" />
            </div>
            <div className="col-span-1">
                <span className="block text-base font-semibold">
                    Fax
                </span>
                <Input placeholder="Reference" />
            </div>
            <div className="col-span-1">
                <span className="block text-base font-semibold">
                    Email
                </span>
                <Input placeholder="Reference" />
            </div>
        </div>
        <div className="flex justify-between gap-5 p-2">
            <div>
              <Button className="text-base bg-blue-200 text-white border-none font-semibold">
                View Security
              </Button>
            </div>
            <div className="flex gap-5">
              <Button className="text-base bg-red-200 text-red-150 border-none font-semibold">
                Cancel
              </Button>
              <Button className="text-base bg-blue-200 text-white border-none font-semibold">
                Save
              </Button>
            </div>
          </div>
        </div>
          </div>
          
      </main>
    </div>
  );
});
