"use client";

import { CustomCard } from "@/app/components/customCard";
import { Button, DatePicker, Form, Radio, Select } from "antd";
import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [form] = Form.useForm();
  return (
    <CustomCard
      cardClassname={"mx-auto"}
      headerContent={<p>Print Chart of Accounts</p>}
    >
      <Form form={form} className="bg-gray-50 py-4 px-8 " layout="vertical">
        <div className="flex justify-end">
          <Form.Item
            name={"branch"}
            className="w-[180px] mb-1"
            label={<p>Branch</p>}
            rules={[{ required: true }]}
          >
            <Select
              allowClear
              optionFilterProp="children"
              className="text-gray-600"
            ></Select>
          </Form.Item>
        </div>
        <div className="flex gap-3 ">
          <Form.Item
            name={"customer"}
            className="flex-1 mb-1"
            label={<p>Customer</p>}
            rules={[{ required: true }]}
          >
            <Select
              allowClear
              optionFilterProp="children"
              className="text-gray-600"
            ></Select>
          </Form.Item>

          <Form.Item
            name={"account"}
            className="flex-1 mb-1"
            label={<p>A/C Number</p>}
            rules={[{ required: true }]}
          >
            <Select
              allowClear
              optionFilterProp="children"
              className="text-gray-600"
            ></Select>
          </Form.Item>
        </div>

        <div className="flex gap-3 ">
          <Form.Item
            name={"level"}
            className="flex-1"
            label={<p>Account level</p>}
            rules={[{ required: true }]}
          >
            <Select
              allowClear
              optionFilterProp="children"
              className="text-gray-600"
            ></Select>
          </Form.Item>

          <Form.Item
            name={"accountType"}
            className="flex-1"
            label={<p>Account Type</p>}
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
              <Radio value={1}>Account Only</Radio>
              <Radio value={2}>Parent Account Only</Radio>
              <Radio value={3}>Account/Parent</Radio>
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
            <Radio.Group>
              <Radio value={1}>Internal GL/Customer</Radio>
              <Radio value={2}>Internal GL</Radio>
              <Radio value={3}>Customer</Radio>
            </Radio.Group>
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
