"use client";
import { DatePicker, Button, Checkbox, Form, Input, Select } from "antd";

const Page = () => {
  return (
    <div className="m-2 overflow-hidden border shadow rounded-lg">
      <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5">
      Sell Trades / Deals - Edit
      </h4>
      <div className="px-3 py-5 bg-gray-50">
        <Form className="p-2 ">
          <div className="grid md:grid-cols-5 gap-5">
            <div className="col-span-2">
              <span className="block text-base font-semibold">Transaction No.</span>
              <Input placeholder="0001" />
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">Date</span>
              <DatePicker format="DD/MM/YYYY" className="w-full" />
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">Ticket</span>
              <Form.Item name="ticket">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">Branch</span>
              <Select
                className="w-full"
                defaultValue="Lagos"
                options={[
                  {
                    value: "001",
                    label: "001",
                  },
                ]}
              />
            </div>
          </div>
          <div className="">
            <div className="md:grid grid-cols-3 gap-5">
              <div className="col-span-1">
                <span className="block text-base font-semibold">Product</span>
                <Select
                  className="w-full"
                  defaultValue="JOHNSON"
                  options={[
                    {
                      value: "001",
                      label: "001",
                    },
                  ]}
                />
              </div>
              <div className="col-span-2">
                <span className="block text-base font-semibold">Subsidiary Account</span>
                <Select
                  className="w-full"
                  defaultValue="JOHNSON"
                  options={[
                    {
                      value: "00059088 |  Abdulazeez",
                      label: "00059088 |  Abdulazeez",
                    },
                  ]}
                />
              </div>
            </div>
            <h5 className="text-green-200 text-xs my-2">
              CSCS A/C: 10113433 | CSCS REG: A909JD334 | Cash Bal: Email:
              abdulazeez@gmail.com | Phone: 08132624118
            </h5>
          </div>
          <div className="grid md:grid-cols-3 gap-5">
            <div className="col-span-1">
              <span className="block text-base font-semibold">Stock Code</span>
              <Select
                className="w-full"
                defaultValue="Lagos"
                options={[
                  {
                    value: "001",
                    label: "001",
                  },
                ]}
              />
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">Quantity</span>
              <Form.Item name="quantity">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">Price</span>
              <Form.Item name="price">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
          </div>
          <div className="grid md:grid-cols-6 gap-5">
            <div className="col-span-2">
              <span className="block text-base font-semibold">Consideration</span>
              <Form.Item name="consideration">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
            <div className="col-span-2">
              <span className="block text-base font-semibold">Bought By</span>
              <Select
                className="w-full"
                defaultValue="JOHNSON"
                options={[
                  {
                    value: "001",
                    label: "001",
                  },
                ]}
              />
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">Sold By</span>
              <Form.Item name="bought by">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
            <div className="col-span-1">
              <span className="block text-base font-semibold">No Of Trans</span>
              <Form.Item name="no of transac">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
          </div>
          <div className="grid md:grid-cols-5 gap-5">
            <div className="col-span-1">
              <span className="block text-base">SEC</span>
              <Form.Item name="sec">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">COMM VAT</span>
              <Form.Item name="comm vat">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">STAMP</span>
              <Form.Item name="stamp">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">NSE</span>
              <Form.Item name="nse">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">COMMISSION</span>
              <Form.Item name="commission">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
          </div>
          <div className="grid md:grid-cols-5 gap-5">
            <div className="col-span-1">
              <span className="block text-base">CSCS</span>
              <Form.Item name="cscs">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">SEC VAT</span>
              <Form.Item name="sec vat">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">NSE VAT</span>
              <Form.Item name="nse vat">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">CSCS VAT</span>
              <Form.Item name="cscs vat">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>{" "}
            <div className="col-span-1">
              <span className="block text-base">SMS ALERT</span>
              <Form.Item name="sms alert">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
          </div>
          <div className="grid md:grid-cols-5 gap-5">
            <div className="col-span-1">
              <span className="block text-base">ALERT VAT</span>
              <Form.Item name="alert vat">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
            <div className="col-span-1">
              <span className="block text-base">TOTAL</span>
              <Form.Item name="total">
                <Input placeholder="2342324" />
              </Form.Item>
            </div>
          </div>
          <div className="flex">
            <Form.Item name="auto post" valuePropName="checked">
              <Checkbox className="mr-5">Auto Post</Checkbox>
            </Form.Item>
            <Form.Item name="unsaved reversal" valuePropName="checked">
              <Checkbox>Unsaved Reversal</Checkbox>
            </Form.Item>
          </div>
          <div className="flex flex-col md:flex md:justify-between md:flex-row gap-5">
            <div className="my-2">
              <Button className=" bg-blue-200 text-white border-none">
                View Unapproved Sales Trades
              </Button>
            </div>
            <div>
              <Button
                htmlType="reset"
                className="bg-red-250 text-red-150 text-base border-none mx-1 md:mx-5"
              >
                Cancel
              </Button>
              <Button
                htmlType="submit"
                className=" bg-blue-200 text-white border-none"
              >
                Save
              </Button>
            </div>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default Page;
