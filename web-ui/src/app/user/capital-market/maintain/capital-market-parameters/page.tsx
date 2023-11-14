"use client";

import { observer } from "mobx-react-lite";
import Link from "next/link";
import { usePathname } from 'next/navigation'
import {Select , Radio , Input, Button} from 'antd'
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });


export default observer(function Page() {
  const pathname = usePathname()
  return (
    <div className={manrope.className}>
  <main className="flex min-h-screen flex-col items-center p-5 gap-16">
     <div className="border rounded-xl shadow bg-gray-50">
     <h4 className="flex justify-start items-center text-base px-10 w-full capitalize bg-[#194bfb] text-white rounded-t-xl h-10">
            <Link
              href="/user/capital-market/maintain/capital-market-parameters"
              className={`${pathname === '/user/capital-market/maintain/capital-market-parameters'} ? bg-gray-40 rounded p-0.5 : 'text-xs text-neutral-800 py-0.5 px-1 border rounded'`}
   
            >
              General
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/trading-account"
              className='text-xs text-white py-0.5 px-1 border rounded'
      >
              Trading Account
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/NGX-Buy"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              NGX Buy
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/NGX-offer"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              NGX Offer
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/minimum-value"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              Minimum Value
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/nasd-buy"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              NASD Buy
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/nasd-offer"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              NASD Offer
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/min-val-nasd"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              Min. Val. NASD
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/bond-buy"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              Bond Buy
            </Link>
            <Link
              href="/user/capital-market/maintain/capital-market-parameters/bond-offer"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              Bond Offer
            </Link>
            <Link
              href="min-val-bond"
              className="text-xs text-white py-0.5 px-1 border rounded"
            >
              Min. Val. Bond
            </Link>
          </h4>
      <div className="px-5 bg-gray-50">
      <div className="grid lg:grid-cols-3 gap-5 my-5">
    <div className="col-span-1">
    <span className="block text-base font-semibold my-2">NGX Product</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
    <span className="block text-base font-semibold my-2">NGX Investment Product</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
    <span className="block text-base font-semibold my-2">NASD Product </span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
      <div className="grid lg:grid-cols-3 gap-5 my-5">
    <div className="col-span-1">
    <span className="block text-base font-semibold my-2">NASD Investment Product</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
    <span className="block text-base font-semibold my-2">Agent Payable Product</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
    <span className="block text-base font-semibold my-2">Bond Investment</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
      <div className="grid lg:grid-cols-3 gap-5 items-center px-3 my-5">
    <div className="col-span-1">
    <span className="block text-base font-semibold my-2">NGX Member Code</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
    <span className="block text-base font-semibold my-2">NASD Member Code</span>
            <Select
              defaultValue="Sterlying Bank 0201"
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
      <span className="block text-gray-500  text-base font-semibold">Commission Type</span>
    <Radio.Group name="radiogroup" className="border shadow-xl w-4/5 flex justify-center py-3 rounded">
    <Radio value='fixed'>Fixed</Radio>
    <Radio value='graduated'>Graduated</Radio>
  </Radio.Group>
    </div>
      </div>
      <div className="grid lg:grid-cols-3 gap-5 my-5">
        <div className="col-span-1">
        <span className="block  text-base font-semibold">No. Of Days Before Mandate Is Droped</span>
          <Input placeholder="Sterlying Bank 0201" />
        </div>
        <div className="col-span-1">
        <span className="block  text-base font-semibold">Price Markup Percentage For Mandate</span>
          <Input placeholder="Sterlying Bank 0201" />
        </div>
        <div className="col-span-1">
        <span className="block text-base font-semibold">Branch</span>
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
      <div className="flex justify-end gap-5 my-5">
      <Button className="text-base bg-red-200 text-red-150 border-none font-semibold">Cancel</Button>
        <Button className="text-base bg-blue-200 text-white border-none font-semibold">Save</Button>
      </div>
      </div>
     
     </div>
    </main>
    </div>
  
  ); 

})