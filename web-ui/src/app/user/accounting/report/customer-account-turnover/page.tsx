"use client";

import { CustomCard } from "@/app/components/customCard";
import {
  Button,
  Checkbox,
  DatePicker,
  Form,
  InputNumber,
  Radio,
  Select,
} from "antd";
import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [form] = Form.useForm();
  return (
    <CustomCard
      cardClassname={"mx-auto"}
      headerContent={<p>Customer Account Turnover</p>}
    >
      <Form form={form} className="bg-gray-50 py-4 px-8 " layout="vertical">
        <div className="flex gap-3 ">
          <Form.Item
            name={"startDate"}
            className="flex-1 mb-1"
            label="From"
            rules={[{ required: true }]}
          >
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>

          <Form.Item
            name={"endDate"}
            className="flex-1 mb-1"
            label="To"
            rules={[{ required: true }]}
          >
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>
        </div>
        <div className="flex gap-3 ">
          <Form.Item
            name={"productAccount"}
            label={"Parent Account"}
            className="flex-1 mb-0"
            rules={[{ required: true }]}
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>

          <Form.Item
            name={"customerAccount"}
            label={"Customer Account"}
            className="flex-1  mb-0"
            rules={[{ required: true }]}
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>
        </div>

        <div>
          <Form.Item
            name={"accountSelection"}
            rules={[{ required: true }]}
            initialValue={1}
            className="mb-0"
          >
            <Radio.Group>
              <Radio value={1}>Credit Balances</Radio>
              <Radio value={2}>Debit Balances</Radio>
              <Radio value={3}>Net Balances</Radio>
            </Radio.Group>
          </Form.Item>
        </div>

        <div className="flex w-5/6 justify-end">
          <Form.Item
            name={"accountSelection4"}
            rules={[{ required: true }]}
            className="mb-0 flex-1 "
          >
            <Checkbox>Consolidated Account</Checkbox>
          </Form.Item>
        </div>

        <div>
          <p>Balances Amount Range</p>
          <div className="flex gap-3">
            <Form.Item
              name={"balanceFrom"}
              label={"From"}
              rules={[{ required: true }]}
              className="mb-0 flex-1"
            >
              <InputNumber className="w-full" />
            </Form.Item>

            <Form.Item
              name={"balanceTo"}
              label={"To"}
              rules={[{ required: true }]}
              className="mb-0 flex-1"
            >
              <InputNumber className="w-full" />
            </Form.Item>
          </div>
        </div>
        <div className="flex flex-wrap">
          <Form.Item
            name={"topBalance"}
            label={"Top Number Of Balance To Show "}
            rules={[{ required: true }]}
            className="flex-1"
          >
            <InputNumber className="w-full" />
          </Form.Item>
        </div>

        <div className="flex justify-end">
          <Button className="bg-blue-200 text-white px-4 py-2 rounded">
            View
          </Button>
        </div>
      </Form>
    </CustomCard>
  );
});
