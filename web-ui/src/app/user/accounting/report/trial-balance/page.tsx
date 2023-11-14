"use client";

import { CustomCard } from "@/app/components/customCard";
import { Button, Checkbox, DatePicker, Form, Radio, Select } from "antd";
import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [form] = Form.useForm();
  return (
    <CustomCard cardClassname={"mx-auto"} headerContent={<p>Trial Balance</p>}>
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
          <Form.Item
            name={"branch"}
            className="flex-1"
            label={"Branch"}
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
              <Radio value={1}>Movement With Closing Balance</Radio>
              <Radio value={2}>Closing Balance Only</Radio>
              <Radio value={3}>
                Opening Balances, Movement With Closing Balances
              </Radio>
            </Radio.Group>
          </Form.Item>
        </div>

        <div className="flex gap-3 ">
          <Form.Item
            name={"parentLevel"}
            label={"Parent A/C"}
            className="flex-1 max-w-[300px]"
            rules={[{ required: true }]}
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>
        </div>

        <div className="flex flex-wrap">
          <Form.Item
            name={"accountSelection1"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Exclude Staff Loan Product Ledger Deals</Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection2"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Order By Asset Type And Sum Each A/C Bal</Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection3"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Exclude Reversal</Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection4"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Exclude Account With Zero Balances</Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection5"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Sum By Individual Account Balances</Checkbox>
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
