"use client";

import { CustomCard } from "@/app/components/customCard";
import { Button, Checkbox, DatePicker, Form, Radio, Select } from "antd";
import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [form] = Form.useForm();
  return (
    <CustomCard
      cardClassname={"mx-auto"}
      headerContent={<p>Customer Statement of Account</p>}
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
            name={"customer"}
            className="flex-1"
            label={"Customer"}
            rules={[{ required: true }]}
          >
            <Select
              allowClear
              optionFilterProp="children"
              className="text-gray-600"
            ></Select>
          </Form.Item>

          <Form.Item
            name={"accountLevel"}
            className="flex-1"
            label="Account Level"
            rules={[{ required: true }]}
          >
            <Select
              allowClear
              optionFilterProp="children"
              className="text-gray-600"
            ></Select>
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
              <Radio value={1}>Exclude Reversal</Radio>
              <Radio value={2}>Include Reversal</Radio>
            </Radio.Group>
          </Form.Item>
        </div>

        <div>
          <Form.Item
            name={"accountSelection1"}
            rules={[{ required: true }]}
            initialValue={1}
            className="mb-0"
          >
            <Checkbox>Exclude Staff Loan Product Ledger Deals</Checkbox>
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
