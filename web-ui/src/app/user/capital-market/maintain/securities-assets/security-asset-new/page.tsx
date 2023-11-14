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
            Securities /Asset-New
          </h4>
          <div className="px-2">
            <div className="grid lg:grid-cols-2 gap-5 my-2">
              <div className="col-span-1">
                <span className="block text-base my-2 font-semibold">Security Code</span>
                <Select
                  defaultValue="009"
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
                <span className="block text-base my-2 font-semibold">Security Name</span>
                <Select
                  defaultValue="009"
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
            <div className="grid lg:grid-cols-2 gap-5">
              <div className="col-span-1 ">
                <span className="block text-base my-2 font-semibold">Sector</span>
                <Select
                  defaultValue="009"
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
              <div className="col-span-1 ">
                <span className="block text-base my-2 font-semibold">Asset Type</span>
                <Select
                  defaultValue="009"
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
            <div className="px-2 border m-4 rounded-xl shadow-md">
              <p className="my-2 text-gray-500 text-base font-bold">
                Equities Details
              </p>
              <div className="grid lg:grid-cols-3 gap-5 my-3">
                <div className="col-span-1">
                  <span className="block text-base my-2 text-gray-500">
                    Registrar
                  </span>
                  <Select
                    defaultValue="009"
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
                  <span className="block text-base my-2 text-gray-500">
                    Nominal Value
                  </span>
                  <Input placeholder="009" />
                </div>
                <div className="col-span-1">
                  <span className="block text-base my-2 text-gray-500">
                    Shares Outstanding
                  </span>
                  <Select
                    defaultValue="009"
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
              <div className="grid lg:grid-cols-3 items-end gap-5 my-5">
                <div className="col-span-1">
                  <span className="block text-base my-2 text-gray-500">
                    CSCS Old Code
                  </span>
                  <Select
                    defaultValue="009"
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
                  <span className="text-gray-500">Delisted By NSE</span>{" "}
                  <Checkbox></Checkbox>
                </div>
              </div>
            </div>
          </div>
          <div className="flex justify-between gap-5 p-5">
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
      </main>
    </div>
  );
});
